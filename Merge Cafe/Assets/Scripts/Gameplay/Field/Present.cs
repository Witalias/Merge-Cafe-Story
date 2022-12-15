using UnityEngine;
using Enums;
using System.Collections.Generic;
using Service;
using System.Linq;
using EventHandlers;

namespace Gameplay.Field
{
    [RequireComponent(typeof(Item))]
    [RequireComponent(typeof(QuickClickTracking))]
    public class Present : MonoBehaviour, IStorable
    {
        private const string ITEM_TYPE_IN_PRESENT_KEY = "ITEM_TYPE_IN_PRESENT";
        private const string ITEM_LEVEL_IN_PRESENT_KEY = "ITEM_LEVEL_IN_PRESENT";
        private const string ITEMS_COUNT_IN_PRESENT_KEY = "ITEMS_COUNT_IN_PRESENT";

        private Item _item;
        private QuickClickTracking _quickClickTracking;

        private readonly List<ItemStorage> _content = new();

        public void Save()
        {
            var hierarchyIndex = _item.CurrentCell.transform.GetSiblingIndex();
            for (var i = 0; i < _content.Count; i++)
            {
                PlayerPrefs.SetInt(ITEM_TYPE_IN_PRESENT_KEY + i + hierarchyIndex, (int)_content[i].Type);
                PlayerPrefs.SetInt(ITEM_LEVEL_IN_PRESENT_KEY + i + hierarchyIndex, _content[i].Level);
            }
            PlayerPrefs.SetInt(ITEMS_COUNT_IN_PRESENT_KEY + hierarchyIndex, _content.Count);
        }

        public void Load()
        {
            var hierarchyIndex = _item.CurrentCell.transform.GetSiblingIndex();
            var contentCount = PlayerPrefs.GetInt(ITEMS_COUNT_IN_PRESENT_KEY + hierarchyIndex, 0);
            for (var i = 0; i < contentCount; ++i)
            {
                var itemType = (ItemType)PlayerPrefs.GetInt(ITEM_TYPE_IN_PRESENT_KEY + i + hierarchyIndex);
                var level = PlayerPrefs.GetInt(ITEM_LEVEL_IN_PRESENT_KEY + i + hierarchyIndex);
                _content.Add(GameStorage.Instanse.GetItem(itemType, level));
            }
        }

        private void Awake()
        {
            _item = GetComponent<Item>();
            _quickClickTracking = GetComponent<QuickClickTracking>();
        }

        private void Start()
        {
            if (GameStorage.Instanse.LoadData && 
                PlayerPrefs.HasKey(ITEMS_COUNT_IN_PRESENT_KEY + _item.CurrentCell.transform.GetSiblingIndex()))
                Load();
            else
            {
                foreach (var item in PresentsInfo.GetContent(_item.Stats.Level))
                {
                    for (var i = 0; i < item.Count; ++i)
                        _content.Add(GameStorage.Instanse.GetItem(item.Type, Random.Range(item.MinLevel, item.MaxLevel + 1)));
                }
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