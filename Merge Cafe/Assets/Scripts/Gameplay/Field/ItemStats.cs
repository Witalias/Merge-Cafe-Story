using UnityEngine;
using Enums;

namespace Gameplay.Field
{
    public class ItemStats
    {
        public Sprite Icon { get; }
        public int Level { get; } = 1;
        public ItemType Type { get; private set; }

        public ItemStats(int level, Sprite icon, ItemType type)
        {
            Icon = icon;
            Level = level;
            Type = type;
        }

        public ItemStats(int level, Sprite icon) : this(level, icon, ItemType.Tea) { }

        public void SetType(ItemType type) => Type = type;
    }
}
