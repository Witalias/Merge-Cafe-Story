using UnityEngine;
using Enums;
using Service;
using System;

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
        public bool Special { get; private set; } = true;
        public bool RewardIsShowing { get; set; } = false;
        public Sound TakeSound { get; }
        public Sound PutSound { get; }

        public static event Action<ItemStorage, float> NewItem;

        public ItemStorage(int level, Sprite icon, ItemType type, bool throwable, bool movable, bool special,
            bool unlocked = false, Sound takeSound = default, Sound putSound = default)
        {
            Icon = icon;
            Level = level;
            Type = type;
            Unlocked = unlocked;
            Throwable = throwable;
            Movable = movable;
            Special = special;
            TakeSound = takeSound;
            PutSound = putSound;

            if (level <= 1)
                Unlock();
        }

        public ItemStorage(ItemStorage other, bool unlocked = true) : this(other.Level, other.Icon, other.Type, other.Throwable, other.Movable, other.Special, unlocked) { }

        public void Unlock()
        {
            Unlocked = true;
        }

        public void UnlockFirstly()
        {
            if (!Unlocked)
                NewItem?.Invoke(this, 2f);
            Unlock();
        }

        public void NotNew()
        {
            IsNew = false;
        }

        public void OpenPresent()
        {
            if (Type != ItemType.Present)
                return;

            Type = ItemType.OpenPresent;
            Icon = GameStorage.Instance.GetItemSprite(Type, Level);
        }

        public bool EqualTo(ItemStorage other) => other.Type == Type && other.Level == Level;
    }
}
