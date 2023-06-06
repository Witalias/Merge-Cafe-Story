using UnityEngine;
using UnityEngine.UI;
using Enums;
using Gameplay.Field;
using System;
using UnityEngine.EventSystems;

namespace Gameplay.Orders
{
    public class OrderPoint : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Image _icon;
        [SerializeField] private Animation _checkMarkCompleted;
        [SerializeField] private Animation _checkMarkAvailable;
        [SerializeField] private Color _availableCellColor;

        private Image _background;
        private Color _defaultCellColor;

        private ItemType _itemType;
        private int _itemLevel;

        public static event Action<ItemType, int> CursorHoveredItemInOrder;
        public static event Action<ItemType, int> CursorExitItemInOrder;

        public Image Background => _background;

        public Image Icon { get => _icon; }

        public Animation CheckMark { get => _checkMarkCompleted; }

        public void SetItem(ItemStorage stats)
        {
            Icon.sprite = stats.Icon;
            _itemType = stats.Type;
            _itemLevel = stats.Level;
        }

        public void SetActiveAvailableMark(bool value)
        {
            if (value && _checkMarkAvailable.gameObject.activeSelf)
                return;

            _checkMarkAvailable.gameObject.SetActive(value);
            if (value) _checkMarkAvailable.Play();
            _background.color = value ? _availableCellColor : _defaultCellColor;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            CursorHoveredItemInOrder?.Invoke(_itemType, _itemLevel);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            CursorExitItemInOrder?.Invoke(_itemType, _itemLevel);
        }

        private void Awake()
        {
            _background = GetComponent<Image>();
            _defaultCellColor = _background.color;
        }
    }

}