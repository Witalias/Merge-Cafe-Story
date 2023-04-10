using UnityEngine;
using System.Collections.Generic;
using Enums;
using Gameplay.Field;
using Gameplay.ItemGenerators;
using System.Linq;

namespace Service
{
    public class GameStorage : MonoBehaviour, IStorable
    {
        public static GameStorage Instance { get; private set; } = null;

        private const string STARS_COUNT_KEY = "STARS_COUNT";
        private const string BRILLIANTS_COUNT_KEY = "BRILLIANTS_COUNT";
        private const string ITEM_IS_NEW_KEY = "ITEM_IS_NEW_";
        private const string ITEM_UNLOCKED_KEY = "ITEM_UNLOCKED_";
        private const string GAME_STAGE_KEY = "GAME_STAGE";
        private const string LANGUAGE_KEY = "LANGUAGE";

        [Header("Settings")]
        [SerializeField] private bool _loadData = false;
        [SerializeField] private int _starsCount = 0;
        [SerializeField] private int _brilliantsCount = 0;
        [SerializeField] private int _generationStarFromLevel = 6;
        [SerializeField] private int[] _starsByItemLevels;
        [SerializeField] private int[] _starsRewardForLevels;
        [SerializeField] private int[] _brilliantsRewardForLevels;
        [SerializeField] private int[] _energyRewardForLevels;
        [SerializeField] private ItemTypeLevel[] _rewardsForNewItems;
        [SerializeField] private int[] _stagesForOrderCounts;

        [Header("Items")]
        [SerializeField] private TypedItemPost[] _typedItems;
        [SerializeField] private ItemCombinations[] _combinations;

        [Header("Sprites")]
        [SerializeField] private Sprite _questionMark;
        [SerializeField] private Sprite _dialogWindow;
        [SerializeField] private Sprite _star;

        [Header("Prefabs")]
        [SerializeField] private GameObject _itemPrefab;
        [SerializeField] private GameObject _starForAnimation;
        [SerializeField] private GameObject _brilliantForAnimation;
        [SerializeField] private GameObject _purchaseButton;
        [SerializeField] private GameObject _sequins;

        private ItemGeneratorStorage _generatorStorage;

        private Cell[] cells;
        private readonly Dictionary<ItemType, ItemStorage[]> _items = new();
        private readonly Dictionary<ItemType, Dictionary<int, Sprite>> _itemSprites = new();

        public static event System.Action NoEmptyCells;

        public Language Language { get; set; } = Language.English;

        public bool LoadData { get => _loadData; }

        public int GameStage { get; set; } = 1;

        public int StarsCount { get => _starsCount; set => _starsCount = value; }

        public int BrilliantsCount { get => _brilliantsCount; set => _brilliantsCount = value; }

        public int GenerationStarFromLevel { get => _generationStarFromLevel; }

        public GameObject ItemPrefab { get => _itemPrefab; }

        public GameObject StarForAnimation { get => _starForAnimation; }

        public GameObject BrilliantForAnimation { get => _brilliantForAnimation; }

        public GameObject PurchaseButton { get => _purchaseButton; }

        public GameObject Sequins { get => _sequins; }

        public Transform ItemsParent { get; private set; }

        public Sprite QuestionMark { get => _questionMark; }

        public Sprite DialogWindow { get => _dialogWindow; }

        public Sprite Star { get => _star; }

        public bool OrdersCountMustBeUpdated { get => System.Array.Exists(_stagesForOrderCounts, element => element == GameStage); }

        public void Save()
        {
            PlayerPrefs.SetInt(STARS_COUNT_KEY, _starsCount);
            PlayerPrefs.SetInt(BRILLIANTS_COUNT_KEY, _brilliantsCount);
            PlayerPrefs.SetInt(GAME_STAGE_KEY, GameStage);
            PlayerPrefs.SetInt(LANGUAGE_KEY, (int)Language);
            foreach (var type in _items.Keys)
            {
                foreach (var item in _items[type])
                {
                    PlayerPrefs.SetInt(ITEM_UNLOCKED_KEY + type + item.Level, item.Unlocked ? 1 : 0);
                    PlayerPrefs.SetInt(ITEM_IS_NEW_KEY + type + item.Level, item.IsNew ? 1 : 0);
                }
            }
        }

        public void Load()
        {
            _starsCount = PlayerPrefs.GetInt(STARS_COUNT_KEY, 0);
            _brilliantsCount = PlayerPrefs.GetInt(BRILLIANTS_COUNT_KEY, 0);
            GameStage = PlayerPrefs.GetInt(GAME_STAGE_KEY, 1);
            Language = (Language)PlayerPrefs.GetInt(LANGUAGE_KEY, 0);
            foreach (var type in _items.Keys)
            {
                foreach (var item in _items[type])
                {
                    if (PlayerPrefs.GetInt(ITEM_UNLOCKED_KEY + type + item.Level, 0) == 1)
                        item.Unlock();
                    if (PlayerPrefs.GetInt(ITEM_IS_NEW_KEY + type + item.Level, 1) == 0)
                        item.NotNew();
                }
            }
        }

        public ItemStorage GetNextItemByAnotherItem(ItemStorage item)
        {
            var nextItemStats = _items[item.Type][item.Level];
            nextItemStats.UnlockFirstly();
            return nextItemStats;
        }

        public ItemStorage GetItem(ItemType type, int level)
        {
            if (_items.ContainsKey(type))
                return _items[type][level - 1];
            return null;
        }

        public bool IsItemMaxLevel(ItemType type, int level)
        {
            if (_generatorStorage.IsGenerator(type))
                return _generatorStorage.IsMaxLevel(type, level);

            if (_items.ContainsKey(type))
                return _items[type].Length == level;
            return true;
        }

        public int GetItemMaxLevel(ItemType itemType)
        {
            if (_items.ContainsKey(itemType))
                return _items[itemType].Length;
            return 1;
        }

        public Sprite GetItemSprite(ItemType type, int level)
        {
            if (_itemSprites.ContainsKey(type))
                return _itemSprites[type][level];
            return null;
        }

        public int GetStarsCountByItemLevel(int level) => _starsByItemLevels[level - 1];

        public int GetStarsRewardByItemLevel(int level) => _starsRewardForLevels[level - 1];

        public int GetBrilliantsRewardByItemlevel(int level) => _brilliantsRewardForLevels[level - 1];

        public int GetEnergyRewardByItemlevel(int level) => _energyRewardForLevels[level - 1];

        public ItemStorage GetRewardForNewItemByLevel(int level) => GetItem(_rewardsForNewItems[level - 1].Type, _rewardsForNewItems[level - 1].Level);

        public Cell GetFirstEmptyCell(bool showMessageIfNoEmpty = false)
        {
            foreach (var cell in cells)
            {
                if (cell.Empty)
                    return cell;
            }
            if (showMessageIfNoEmpty)
                NoEmptyCells?.Invoke();
            return null;
        }

        public Cell GetRandomEmptyCell(bool showMessageIfNoEmpty = false)
        {
            var emptyCells = new List<Cell>();
            foreach (var cell in cells)
            {
                if (cell.Empty)
                    emptyCells.Add(cell);
            }
            if (emptyCells.Count == 0)
            {
                if (showMessageIfNoEmpty)
                    NoEmptyCells?.Invoke();
                return null;
            }
            return emptyCells[Random.Range(0, emptyCells.Count)];
        }

        public Item[] GetItemsOnField(ItemType type, int level, int count)
        {
            if (count <= 0)
                return new Item[0];

            var item = GetItem(type, level);
            var results = new List<Item>();
            foreach (var cell in cells)
            {
                if (cell.Empty)
                    continue;
                if (cell.Item.Stats.EqualTo(item))
                    results.Add(cell.Item);
                if (results.Count == count)
                    break;
            }
            return results.ToArray();
        }

        public Sound GetCombinateSound(ItemType current, ItemType with)
        {
            foreach (var combination in _combinations)
            {
                if (ExistsCombination(combination, current, with))
                    return combination.CombinateSound;
            }
            return Sound.None;
        }

        public ItemType GetSecondItemTypeInCombination(ItemType type)
        {
            foreach (var combination in _combinations)
            {
                if (combination.FirstType == type)
                    return combination.SecondType;
                else if (combination.SecondType == type)
                    return combination.FirstType;
            }
            throw new System.ArgumentException("Нет пары для типа " + type.ToString());
        }

        public bool FieldHasItem(ItemStorage item, bool highlight = true)
        {
            var result = false;
            foreach (var cell in cells)
            {
                if (cell.Item == null)
                    continue;

                if (cell.Item.Stats.EqualTo(item))
                {
                    result = true;
                    cell.Item.SetActiveParticles(highlight);
                }
            }
            return result;
        }

        public bool HasEmptyCells(bool showMessageIfNoEmpty = false) => GetFirstEmptyCell(showMessageIfNoEmpty) != null;

        public bool IsCombinatingWith(ItemType current, ItemType with)
        {
            foreach (var combination in _combinations)
            {
                if (ExistsCombination(combination, current, with))
                    return true;
            }
            return false;
        }

        public void IncrementGameStage() => ++GameStage;

        public void RemoveItemsHighlight()
        {
            foreach (var cell in cells)
            {
                if (cell.Item == null)
                    continue;

                cell.Item.SetActiveParticles(false);
            }
        }

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);

            ItemsParent = GetObjectByTag(Tags.ItemsParent).transform;
            cells = GetObjectByTag(Tags.CellsParent).GetComponentsInChildren<Cell>();
            _generatorStorage = GetObjectByTag(Tags.ItemGeneratorStorage).GetComponent<ItemGeneratorStorage>();

            CreateItemsDictionary();
            CreateItemSpritesDictionary();
        }

        private void Start()
        {
            if (_loadData)
                Load();
        }

        private bool ExistsCombination(ItemCombinations combination, ItemType first, ItemType second)
        {
            return (first == combination.FirstType && second == combination.SecondType) ||
                (first == combination.SecondType && second == combination.FirstType);
        }

        private GameObject GetObjectByTag(Tags tag) => GameObject.FindGameObjectWithTag(tag.ToString());

        private void CreateItemSpritesDictionary()
        {
            foreach (var element in _items)
            {
                var spritesDict = new Dictionary<int, Sprite>();

                foreach (var item in element.Value)
                {
                    //item.SetType(element.Key);
                    spritesDict.Add(item.Level, item.Icon);
                }

                _itemSprites.Add(element.Key, spritesDict);
            }
        }

        private void CreateItemsDictionary()
        {
            foreach (var item in _typedItems)
                _items.Add(item.Type, GetItemStorageArray(item));
        }

        private ItemStorage[] GetItemStorageArray(TypedItemPost item)
        {
            var itemStats = new ItemStorage[item.Items.Length];
            for (var i = 0; i < itemStats.Length; ++i)
                itemStats[i] = new ItemStorage(i + 1, item.Items[i].Icon, item.Type, item.Throwable, 
                    item.Movable, false, item.Items[i].TakeSound, item.Items[i].PutSound);
            return itemStats;
        }
    }
}