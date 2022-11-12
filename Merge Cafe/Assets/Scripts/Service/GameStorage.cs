using UnityEngine;
using System.Collections.Generic;
using Enums;
using Gameplay.Field;
using Gameplay.ItemGenerators;
using System.Linq;

namespace Service
{
    public class GameStorage : MonoBehaviour
    {
        public static GameStorage Instanse { get; private set; } = null;

        [Header("Settings")]
        [SerializeField] [Range(1, 2)] private int _gameStage = 1;
        [SerializeField] private int _starsCount = 0;
        [SerializeField] private int _brilliantsCount = 0;
        [SerializeField] private int _generationStarFromLevel = 6;
        [SerializeField] private int[] _starsByItemLevels;
        [SerializeField] private int[] _starsRewardForLevels;
        [SerializeField] private int[] _brilliantsRewardForLevels;
        [SerializeField] private ItemTypeLevel[] _rewardsForNewItems;
        [SerializeField] private int[] _stagesForOrderCounts;

        [Header("Items")]
        [SerializeField] private TypedItem[] _typedItems;
        [SerializeField] private ItemCombinations[] _combinations;

        [Header("Sprites")]
        [SerializeField] private Sprite _questionMark;
        [SerializeField] private Sprite _dialogWindow;

        [Header("Prefabs")]
        [SerializeField] private GameObject _itemPrefab;
        [SerializeField] private GameObject _starForAnimation;
        [SerializeField] private GameObject _brilliantForAnimation;
        [SerializeField] private GameObject _purchaseButton;
        [SerializeField] private GameObject _sequins;

        private ItemGeneratorStorage _generatorStorage;

        private Cell[] cells;
        private Dictionary<ItemType, ItemStorage[]> _items = new Dictionary<ItemType, ItemStorage[]>();
        private Dictionary<ItemType, Dictionary<int, Sprite>> _itemSprites = new Dictionary<ItemType, Dictionary<int, Sprite>>();

        public static event System.Action NoEmptyCells;

        public int GameStage { get => _gameStage; set => _gameStage = value; }

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

        public bool OrdersCountMustBeUpdated { get => System.Array.Exists(_stagesForOrderCounts, element => element == GameStage); }

        public ItemStorage GetNextItemByAnotherItem(ItemStorage item)
        {
            var nextItemStats = _items[item.Type][item.Level];
            nextItemStats.Unlock();
            return nextItemStats;
        }

        public ItemStorage GetItem(ItemType type, int level)
        {
            if (_items.ContainsKey(type))
                return _items[type][level - 1];
            return null;
        }

        public bool IsItemMaxLevel(ItemStorage item)
        {
            if (_generatorStorage.IsGenerator(item.Type))
                return _generatorStorage.IsMaxLevel(item);

            if (_items.ContainsKey(item.Type))
                return _items[item.Type].Length == item.Level;
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

        public void IncrementGameStage() => ++_gameStage;

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
            if (Instanse == null)
                Instanse = this;
            else
                Destroy(gameObject);

            DontDestroyOnLoad(gameObject);

            ItemsParent = GetObjectByTag(Tags.ItemsParent).transform;
            cells = GetObjectByTag(Tags.CellsParent).GetComponentsInChildren<Cell>();
            _generatorStorage = GetObjectByTag(Tags.ItemGeneratorStorage).GetComponent<ItemGeneratorStorage>();

            CreateItemsDictionary();
            CreateItemSpritesDictionary();
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

        private ItemStorage[] GetItemStorageArray(TypedItem item)
        {
            var itemStats = new ItemStorage[item.Icons.Length];
            for (var i = 0; i < itemStats.Length; ++i)
                itemStats[i] = new ItemStorage(i + 1, item.Icons[i], item.Type, item.Throwable, item.Movable);
            return itemStats;
        }
    }

    [System.Serializable]
    public class TypedItem
    {
        public ItemType Type;
        public bool Throwable = true;
        public bool Movable = true;
        public Sprite[] Icons;
    }
}