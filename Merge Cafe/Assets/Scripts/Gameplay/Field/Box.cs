using Service;
using System;
using UnityEngine;

namespace Gameplay.Field
{
    [RequireComponent(typeof(QuickClickTracking))]
    [RequireComponent(typeof(Item))]
    public class Box : MonoBehaviour
    {
        private QuickClickTracking _clickTracking;
        private Item _item;

        public static event Func<ItemStorage> GetRandomOrderItem;
        public static event Func<ItemStorage> GetOrderItemMaxLevel;
        public static event Action NoOrderPoints;
        public static event Action Opened;

        private void Awake()
        {
            _clickTracking = GetComponent<QuickClickTracking>();
            _item = GetComponent<Item>();
        }

        private void Update()
        {
            if (_clickTracking.QuickClicked)
                Open();
        }

        private void Open()
        {
            ItemStorage item = null;
            if (_item.Stats.Level == 2)
                item = GetRandomOrderItem?.Invoke();
            else if (_item.Stats.Level > 2)
                item = GetOrderItemMaxLevel?.Invoke();
            else return;

            if (item == null)
            {
                NoOrderPoints?.Invoke();
                return;
            }
            if (!GameStorage.Instance.HasEmptyCells(true))
                return;

            _item.Remove();
            GameStorage.Instance.GetRandomEmptyCell().CreateItem(item, transform.position);
            item.UnlockFirstly();
            Opened?.Invoke();
        }
    }
}