using UnityEngine;
using Enums;
using System.Collections.Generic;
using Service;
using System.Linq;

namespace Gameplay.Field
{
    [RequireComponent(typeof(Item))]
    [RequireComponent(typeof(QuickClickTracking))]
    public class Present : MonoBehaviour
    {
        private Item _item;
        private QuickClickTracking _quickClickTracking;

        private List<ItemStorage> _content = new List<ItemStorage>();

        private void Awake()
        {
            _item = GetComponent<Item>();
            _quickClickTracking = GetComponent<QuickClickTracking>();

            foreach (var item in PresentsInfo.GetContent(_item.Stats.Level))
            {
                for (var i = 0; i < item.Count; ++i)
                    _content.Add(GameStorage.Instanse.GetItem(item.Type, Random.Range(item.MinLevel, item.MaxLevel + 1)));
            }
        }

        private void Update()
        {
            if (_quickClickTracking.QuickClicked)
            {
                if (_item.Stats.Type == ItemType.Present)
                    OpenPresent();
                else if (_item.Stats.Type == ItemType.OpenPresent)
                    GetItem();
            }
        }

        private void OpenPresent()
        {
            _item.OpenPresent();
        }

        private void GetItem()
        {
            if (!GameStorage.Instanse.HasEmptyCells(true))
                return;

            var nextItem = _content[Random.Range(0, _content.Count)];
            _content.Remove(nextItem);

            if (_content.Count == 0)
                _item.Remove();

            GameStorage.Instanse.GetRandomEmptyCell(true).CreateItem(nextItem, transform.position);
        }
    }
}