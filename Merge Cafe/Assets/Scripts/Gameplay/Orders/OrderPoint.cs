using UnityEngine;
using UnityEngine.UI;
using Enums;
using Gameplay.Field;
using System;

namespace Gameplay.Orders
{
    public class OrderPoint : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private Animation _checkMark;

        private ItemType _itemType;

        public static event Action<ItemType> CursorHoveredItemInOrder;

        public Image Icon { get => _icon; }

        public Animation CheckMark { get => _checkMark; }

        public void SetItem(ItemStats stats)
        {
            Icon.sprite = stats.Icon;
            _itemType = stats.Type;
        }

        private void OnMouseEnter()
        {
            CursorHoveredItemInOrder?.Invoke(_itemType);
        }
    }

}