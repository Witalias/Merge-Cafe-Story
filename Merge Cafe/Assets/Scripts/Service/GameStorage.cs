using UnityEngine;
using System.Collections.Generic;
using Enums;
using Gameplay.Field;
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

        [Header("items")]
        [SerializeField] private TypedItem[] _typedItems;

        [Header("Sprites")]
        [SerializeField] private Sprite _questionMark;

        [Header("Prefabs")]
        [SerializeField] private GameObject _itemPrefab;
        [SerializeField] private GameObject _starForAnimation;
        [SerializeField] private GameObject _brilliantForAnimation;

        private Cell[] cells;
        private Dictionary<ItemType, ItemStorage[]> _items = new Dictionary<ItemType, ItemStorage[]>();
        private Dictionary<ItemType, Dictionary<int, Sprite>> _itemSprites = new Dictionary<ItemType, Dictionary<int, Sprite>>();

        public static event System.Action NoEmptyCells;

        public int GameStage { get => _gameStage; set => _gameStage = value; }

        public int StatsCount { get => _starsCount; set => _starsCount = value; }

        public int BrilliantsCount { get => _brilliantsCount; set => _brilliantsCount = value; }

        public int GenerationStarFromLevel { get => _generationStarFromLevel; }

        public GameObject ItemPrefab { get => _itemPrefab; }

        public GameObject StarForAnimation { get => _starForAnimation; }

        public GameObject BrilliantForAnimation { get => _brilliantForAnimation; }

        public Transform ItemsParent { get; private set; }

        public Sprite QuestionMark { get => _questionMark; }

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

            CreateItemsDictionary();
            CreateItemSpritesDictionary();
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
                itemStats[i] = new ItemStorage(i + 1, item.Icons[i], item.Type, item.Throwable);
            return itemStats;
        }
    }

    [System.Serializable]
    public class TypedItem
    {
        public ItemType Type;
        public bool Throwable;
        public Sprite[] Icons;
    }
}