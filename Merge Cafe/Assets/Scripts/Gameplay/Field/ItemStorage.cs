using UnityEngine;
using Enums;
using Service;

namespace Gameplay.Field
{
    public class ItemStorage
    {
        public Sprite Icon { get; private set; }
        public int Level { get; } = 1;
        public ItemType Type { get; private set; }
        public bool Unlocked { get; private set; } = false;
        public bool IsNew { get; private set; } = true;
        public bool Throwable { get; private set; } = true;
        public bool Movable { get; private set; } = true;

        public ItemStorage(int level, Sprite icon, ItemType type, bool throwable, bool movable, bool unlocked = false)
        {
            Icon = icon;
            Level = level;
            Type = type;
            Unlocked = unlocked;
            Throwable = throwable;
            Movable = movable;

            if (level <= 1)
                Unlock();
        }

        public ItemStorage(ItemStorage other, bool unlocked = true) : this(other.Level, other.Icon, other.Type, other.Throwable, other.Movable, unlocked) { }

        public void Unlock() => Unlocked = true;

        public void NotNew() => IsNew = false;

        public void OpenPresent()
        {
            if (Type != ItemType.Present)
                return;

            Type = ItemType.OpenPresent;
            Icon = GameStorage.Instanse.GetItemSprite(Type, Level);
        }

        public bool EqualTo(ItemStorage other) => other.Type == Type && other.Level == Level;
    }
}
