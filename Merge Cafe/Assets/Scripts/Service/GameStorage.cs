using UnityEngine;
using System.Collections.Generic;
using Enums;
using Gameplay.Field;

namespace Service
{
    public class GameStorage : MonoBehaviour
    {
        public static GameStorage Instanse { get; private set; } = null;

        [Header("Settings")]
        [SerializeField] [Range(1, 2)] private int _gameStage = 1;
        [SerializeField] private int _starsCount = 0;

        [Header("Sprites")]
        [SerializeField] private Sprite _questionMark;
        [SerializeField] private Sprite[] _teaIcons;
        [SerializeField] private Sprite[] _coffeeIcons;

        [Header("Prefabs")]
        [SerializeField] private GameObject _itemPrefab;

        private Cell[] cells;
        private Dictionary<ItemType, ItemStats[]> _items;
        private Dictionary<ItemType, Dictionary<int, Sprite>> _itemSprites = new Dictionary<ItemType, Dictionary<int, Sprite>>();

        public int GameStage { get => _gameStage; }

        public int StatsCount { get => _starsCount; set => _starsCount = value; }

        public GameObject ItemPrefab { get => _itemPrefab; }

        public Transform ItemsParent { get; private set; }

        public Sprite QuestionMark { get => _questionMark; }

        public ItemStats GetNextItemByAnotherItem(ItemStats item)
        {
            var nextItemStats = _items[item.Type][item.Level];
            nextItemStats.Unlock();
            return nextItemStats;
        }

        public ItemStats GetItem(ItemType type, int level) => _items[type][level - 1];

        public bool IsItemMaxLevel(ItemStats item) => _items[item.Type].Length == item.Level;

        public int GetItemMaxLevel(ItemType itemType) => _items[itemType].Length;

        public Sprite GetItemSprite(ItemType type, int level) => _itemSprites[type][level];

        public Cell GetEmptyCell()
        {
            foreach (var cell in cells)
            {
                if (cell.Empty)
                    return cell;
            }
            return null;
        }

        public void IncrementGameStage() => ++_gameStage;

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
                    item.SetType(element.Key);
                    spritesDict.Add(item.Level, item.Icon);
                }

                _itemSprites.Add(element.Key, spritesDict);
            }
        }

        private void CreateItemsDictionary()
        {
            _items = new Dictionary<ItemType, ItemStats[]>
            {
                [ItemType.Tea] = GetItemStatsArray(_teaIcons),
                [ItemType.Coffee] = GetItemStatsArray(_coffeeIcons),
            };
        }

        private ItemStats[] GetItemStatsArray(Sprite[] sprites)
        {
            var itemStats = new ItemStats[sprites.Length];
            for (var i = 0; i < itemStats.Length; ++i)
                itemStats[i] = new ItemStats(i + 1, sprites[i]);
            return itemStats;
        }
    }

}