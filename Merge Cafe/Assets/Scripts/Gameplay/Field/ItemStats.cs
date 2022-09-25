using UnityEngine;
using Enums;
using Service;

namespace Gameplay.Field
{
    public class ItemStats
    {
        public Sprite Icon { get; private set; }
        public int Level { get; } = 1;
        public ItemType Type { get; private set; }
        public bool Unlocked { get; private set; } = false;
        public bool IsNew { get; private set; } = true;

        public ItemStats(int level, Sprite icon, ItemType type, bool unlocked = false)
        {
            Icon = icon;
            Level = level;
            Type = type;
            Unlocked = unlocked;

            if (level <= 1)
                Unlock();
        }

        public ItemStats(int level, Sprite icon) : this(level, icon, ItemType.Tea) { }

        public ItemStats(ItemStats other, bool unlocked = true) : this(other.Level, other.Icon, other.Type, unlocked) { }

        public void SetType(ItemType type) => Type = type;

        public void Unlock() => Unlocked = true;

        public void NotNew() => IsNew = false;

        public void OpenPresent()
        {
            if (Type != ItemType.Present)
                return;

            Type = ItemType.OpenPresent;
            Icon = GameStorage.Instanse.GetItemSprite(Type, Level);
        }

        public bool EqualTo(ItemStats other) => other.Type == Type && other.Level == Level;
    }
}
