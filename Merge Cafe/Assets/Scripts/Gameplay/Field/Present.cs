using UnityEngine;
using Enums;

namespace Gameplay.Field
{
    [RequireComponent(typeof(Item))]
    public class Present : MonoBehaviour
    {
        private Item _item;

        private void Awake()
        {
            _item = GetComponent<Item>();
        }

        private void OnMouseDown()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (_item.Stats.Type == ItemType.Present)
                    _item.Stats.OpenPresent();
            }
        }
    }
}