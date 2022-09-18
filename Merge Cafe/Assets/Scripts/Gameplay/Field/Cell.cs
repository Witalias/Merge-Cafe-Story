using UnityEngine;
using Enums;
using Service;

namespace Gameplay.Field
{
    public class Cell : MonoBehaviour
    {
        [SerializeField] private bool empty = true;
        [SerializeField] private ItemType itemType;
        [SerializeField] private int level;

        private GameStorage storage;

        public Item Item { get; private set; } = null;

        public bool Empty { get => Item == null; }

        public void Clear() => Item = null;

        public void SetItem(Item item)
        {
            CheckEmptiness();
            Item = item;
            Item.SetCell(this);
        }

        public void CreateItem(ItemStats stats)
        {
            CreateItem(stats, transform.position);
        }

        public void CreateItem(ItemStats stats, Vector2 position)
        {
            CheckEmptiness();
            Item = Instantiate(storage.ItemPrefab, position, Quaternion.identity, storage.ItemsParent)
                .GetComponent<Item>();
            Item.Initialize(stats);
            Item.SetCell(this);
            Item.ReturnToCell();
        }

        private void Start()
        {
            storage = GameStorage.Instanse;

            if (!empty)
            {
                var maxLevel = storage.GetItemMaxLevel(itemType);
                level = Mathf.Clamp(level, 1, maxLevel);
                CreateItem(new ItemStats(level, storage.GetItemSprite(itemType, level), itemType));
            }
        }

        private void CheckEmptiness()
        {
            if (Item != null)
                Debug.LogError("You're trying to put an item in an occupied cell");
        }
    }
}
