using UnityEngine;
using Enums;
using System.Collections.Generic;
using Service;
using System.Linq;
using EventHandlers;
using Gameplay.Tutorial;

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
        private int _currentCellIndex;

        private readonly List<ItemStorage> _content = new();

        public static event System.Action Opened;
        public static event System.Action ClickedOpen;

        public void Save()
        {
            var hierarchyIndex = _currentCellIndex;
            for (var i = 0; i < _content.Count; i++)
            {
                PlayerPrefs.SetInt(ITEM_TYPE_IN_PRESENT_KEY + i + hierarchyIndex, (int)_content[i].Type);
                PlayerPrefs.SetInt(ITEM_LEVEL_IN_PRESENT_KEY + i + hierarchyIndex, _content[i].Level);
            }
            PlayerPrefs.SetInt(ITEMS_COUNT_IN_PRESENT_KEY + hierarchyIndex, _content.Count);
        }

        public void Load()
        {
            var hierarchyIndex = _currentCellIndex;
            var contentCount = PlayerPrefs.GetInt(ITEMS_COUNT_IN_PRESENT_KEY + hierarchyIndex, 0);
            for (var i = 0; i < contentCount; ++i)
            {
                var itemType = (ItemType)PlayerPrefs.GetInt(ITEM_TYPE_IN_PRESENT_KEY + i + hierarchyIndex);
                var level = PlayerPrefs.GetInt(ITEM_LEVEL_IN_PRESENT_KEY + i + hierarchyIndex);
                _content.Add(GameStorage.Instance.GetItem(itemType, level));
            }
            PlayerPrefs.DeleteKey(ITEMS_COUNT_IN_PRESENT_KEY + hierarchyIndex);
        }

        private void Awake()
        {
            _item = GetComponent<Item>();
            _quickClickTracking = GetComponent<QuickClickTracking>();
        }

        private void OnEnable()
        {
            Item.ItemRemoved += DeleteSave;
            Item.CellChanged += ChangeCell;
        }

        private void Start()
        {
            _currentCellIndex = _item.CurrentCell.transform.GetSiblingIndex();
            if (GameStorage.Instance.LoadData && 
                PlayerPrefs.HasKey(ITEMS_COUNT_IN_PRESENT_KEY + _currentCellIndex))
                Load();
            else
            {
                foreach (var item in PresentsInfo.GetContent(_item.Stats.Level))
                {
                    for (var i = 0; i < item.Count; ++i)
                        _content.Add(GameStorage.Instance.GetItem(item.Type, Random.Range(item.MinLevel, item.MaxLevel + 1)));
                }
                Save();
            }
        }

        private void Update()
        {
            if (_quickClickTracking.QuickClicked && (TutorialSystem.TutorialDone || GetComponent<TutorialTarget>() != null))
            {
                if (_item.Stats.Type == ItemType.Present)
                    OpenPresent();
                else if (_item.Stats.Type == ItemType.OpenPresent)
                    GetItem();
            }
        }

        private void OnDisable()
        {
            Item.ItemRemoved -= DeleteSave;
            Item.CellChanged -= ChangeCell;
        }

        private void OpenPresent()
        {
            _item.OpenPresent();
            Opened?.Invoke();
        }

        private void GetItem()
        {
            ClickedOpen?.Invoke();
            if (!GameStorage.Instance.HasEmptyCells(true))
                return;

            var nextItem = _content[Random.Range(0, _content.Count)];
            _content.Remove(nextItem);

            if (_content.Count == 0)
            {
                _item.Remove();
                DeleteSave(_currentCellIndex);
            }
            else
                Save();

            GameStorage.Instance.GetRandomEmptyCell(true).CreateItem(nextItem, transform.position);
        }

        private void DeleteSave(int cellIndex)
        {
            if (cellIndex != _currentCellIndex)
                return;

            PlayerPrefs.DeleteKey(ITEMS_COUNT_IN_PRESENT_KEY + cellIndex);
        }

        private void ChangeCell(int from, int to)
        {
            if (from != _currentCellIndex)
                return;

            DeleteSave(from);
            _currentCellIndex = to;
            Save();
        }
    }
}