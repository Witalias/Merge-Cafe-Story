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

        public void CreateItem(ItemStorage stats)
        {
            CreateItem(stats, transform.position);
        }

        public void CreateItem(ItemStorage stats, Vector2 position)
        {
            CheckEmptiness();
            Item = Instantiate(storage.ItemPrefab, position, Quaternion.identity, storage.ItemsParent)
                .GetComponent<Item>();
            Item.Initialize(stats);
            Item.SetCell(this);
            Item.ReturnToCell();

            switch (stats.Type)
            {
                case ItemType.Present:
                case ItemType.OpenPresent:
                    Item.gameObject.AddComponent(typeof(Present));
                    break;
                case ItemType.Star:
                    var starCurrency = Item.gameObject.AddComponent(typeof(ItemCurrency)) as ItemCurrency;
                    starCurrency.SetType(CurrencyType.Star);
                    break;
                case ItemType.Brilliant:
                    var brilliantCurrency = Item.gameObject.AddComponent(typeof(ItemCurrency)) as ItemCurrency;
                    brilliantCurrency.SetType(CurrencyType.Brilliant);
                    break;
            }
        }

        private void Start()
        {
            storage = GameStorage.Instanse;

            if (!empty)
            {
                var maxLevel = storage.GetItemMaxLevel(itemType);
                level = Mathf.Clamp(level, 1, maxLevel);
                CreateItem(new ItemStorage(storage.GetItem(itemType, level)));
            }
        }

        private void CheckEmptiness()
        {
            if (Item != null)
                Debug.LogError("You're trying to put an item in an occupied cell");
        }
    }
}
