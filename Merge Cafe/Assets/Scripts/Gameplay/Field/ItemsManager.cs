using Enums;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Field
{
    public class ItemsManager : MonoBehaviour
    {
        public static ItemsManager Instance { get; private set; }

        private readonly Dictionary<ItemType, Dictionary<int, List<Item>>> _storage = new();

        public void Add(Item item)
        {
            var stats = item.Stats;
            if (_storage.ContainsKey(stats.Type))
            {
                if (_storage[stats.Type].ContainsKey(stats.Level))
                    _storage[stats.Type][stats.Level].Add(item);
                else
                    _storage[stats.Type].Add(stats.Level, new List<Item> { item });
            }
            else
            {
                _storage.Add(stats.Type, new Dictionary<int, List<Item>> { [stats.Level] = new List<Item> { item } });
            }
        }

        public void Remove(Item item)
        {
            _storage[item.Stats.Type][item.Stats.Level].Remove(item);
            foreach (var _item in _storage[item.Stats.Type][item.Stats.Level])
                _item.SetActiveMark(false);
        }

        public void SetMarksOn(ItemType type, int level, bool value)
        {
            if (!_storage.ContainsKey(type) || !_storage[type].ContainsKey(level))
                return;

            foreach (var item in _storage[type][level])
                item.SetActiveMark(value);
        }

        public bool IsAvailable(ItemType type, int level)
        {
            if (!_storage.ContainsKey(type) || !_storage[type].ContainsKey(level) || _storage[type][level].Count == 0)
                return false;
            foreach (var item in _storage[type][level])
                item.SetActiveMark(true);
            return true;
        }

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }
    }
}