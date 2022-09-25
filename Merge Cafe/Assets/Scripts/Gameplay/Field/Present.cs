using UnityEngine;
using Enums;
using System;

namespace Gameplay.Field
{
    [RequireComponent(typeof(Item))]
    [RequireComponent(typeof(QuickClickTracking))]
    public class Present : MonoBehaviour
    {
        private Item _item;
        private QuickClickTracking _quickClickTracking;

        private void Awake()
        {
            _item = GetComponent<Item>();
            _quickClickTracking = GetComponent<QuickClickTracking>();
        }

        private void Update()
        {
            if (_quickClickTracking.QuickClicked)
            {
                OpenPresent();
            }
        }

        private void OpenPresent()
        {
            if (_item.Stats.Type == ItemType.Present)
                _item.OpenPresent();
        }

        //private void OnMouseDown()
        //{
        //    if (_item.Stats.Type == ItemType.Present)
        //        PresentClickedLeftMouseButton?.Invoke();
        //}
    }
}