using UnityEngine;
using Enums;
using Service;
using UnityEngine.UI;

namespace Gameplay.Field
{
    public class Cell : MonoBehaviour
    {
        [SerializeField] private bool empty = true;
        [SerializeField] private ItemType itemType;
        [SerializeField] private int level;
        [SerializeField] private Color _lockColor;

        private GameStorage _storage;
        private Image _image;
        private Color _initialColor;

        public Item Item { get; private set; } = null;

        public bool Empty { get => Item == null; }

        public void Clear()
        {
            Item = null;
            SetColor(_initialColor);
        }

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
            if (!stats.Movable)
                SetColor(_lockColor);

            CheckEmptiness();
            Item = Instantiate(_storage.ItemPrefab, position, Quaternion.identity, _storage.ItemsParent)
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

        private void Awake()
        {
            _image = GetComponent<Image>();
            _initialColor = _image.color;
        }

        private void Start()
        {
            _storage = GameStorage.Instanse;

            if (!empty)
            {
                var maxLevel = _storage.GetItemMaxLevel(itemType);
                level = Mathf.Clamp(level, 1, maxLevel);
                CreateItem(new ItemStorage(_storage.GetItem(itemType, level)));
            }
        }

        private void CheckEmptiness()
        {
            if (Item != null)
                Debug.LogError("You're trying to put an item in an occupied cell");
        }

        private void SetColor(Color color) => _image.color = color;
    }
}
