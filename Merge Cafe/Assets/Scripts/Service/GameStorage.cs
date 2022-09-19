using UnityEngine;
using System.Collections.Generic;
using Enums;
using Gameplay.Field;

namespace Service
{
    public class GameStorage : MonoBehaviour
    {
        public static GameStorage Instanse { get; private set; } = null;

        [Header("Sprites")]
        [SerializeField] private Sprite questionMark;
        [SerializeField] private Sprite[] teaIcons;
        [SerializeField] private Sprite[] coffeeIcons;

        [Header("Prefabs")]
        [SerializeField] private GameObject itemPrefab;

        private Cell[] cells;
        private Dictionary<ItemType, ItemStats[]> items;
        private Dictionary<ItemType, Dictionary<int, Sprite>> itemSprites = new Dictionary<ItemType, Dictionary<int, Sprite>>();

        public GameObject ItemPrefab { get => itemPrefab; }

        public Transform ItemsParent { get; private set; }

        public Sprite QuestionMark { get => questionMark; }

        public ItemStats GetNextItemByAnotherItem(ItemStats item)
        {
            var nextItemStats = items[item.Type][item.Level];
            nextItemStats.Unlock();
            return nextItemStats;
        }

        public ItemStats GetItem(ItemType type, int level) => items[type][level - 1];

        public bool IsItemMaxLevel(ItemStats item) => items[item.Type].Length == item.Level;

        public int GetItemMaxLevel(ItemType itemType) => items[itemType].Length;

        public Sprite GetItemSprite(ItemType type, int level) => itemSprites[type][level];

        public Cell GetEmptyCell()
        {
            foreach (var cell in cells)
            {
                if (cell.Empty)
                    return cell;
            }
            return null;
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
            foreach (var element in items)
            {
                var spritesDict = new Dictionary<int, Sprite>();

                foreach (var item in element.Value)
                {
                    item.SetType(element.Key);
                    spritesDict.Add(item.Level, item.Icon);
                }

                itemSprites.Add(element.Key, spritesDict);
            }
        }

        private void CreateItemsDictionary()
        {
            items = new Dictionary<ItemType, ItemStats[]>
            {
                [ItemType.Tea] = GetItemStatsArray(teaIcons),
                [ItemType.Coffee] = GetItemStatsArray(coffeeIcons),
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