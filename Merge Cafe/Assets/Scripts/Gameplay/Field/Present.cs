using UnityEngine;
using Enums;
using System;

namespace Gameplay.Field
{
    [RequireComponent(typeof(Item))]
    public class Present : MonoBehaviour
    {
        private Item _item;

        public static event Action PresentClickedLeftMouseButton;

        private void Awake()
        {
            _item = GetComponent<Item>();
        }

        private void OnMouseOver()
        {
            if (Input.GetMouseButtonDown(1))
            {
                if (_item.Stats.Type == ItemType.Present)
                    _item.OpenPresent();
            }
        }

        private void OnMouseDown()
        {
            if (_item.Stats.Type == ItemType.Present)
                PresentClickedLeftMouseButton?.Invoke();
        }
    }
}