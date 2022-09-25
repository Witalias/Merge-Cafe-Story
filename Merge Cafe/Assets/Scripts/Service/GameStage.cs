using Enums;

namespace Service
{
    public static class GameStage
    {
        private static readonly Settings[] _storage;

        static GameStage()
        {
            _storage = new[]
            {
                new Settings(new[]
                {
                    (ItemType.Tea, 1, 3, 1),
                    (ItemType.Coffee, 1, 3, 1),
                }, new[]
                {
                    (ItemType.Tea, 4),
                }, new[]
                {
                    (ItemType.Present, 1, 2),
                    (ItemType.Present, 1, 3),
                }, 1)
            };
        }

        public static Settings GetSettingsByStage(int value)
        {
            if (value > _storage.Length)
                return null;
            return _storage[value - 1];
        }

        public class Settings
        {
            public (ItemType Type, int MinLevel, int MaxLevel, int RewardLevel)[] Items { get; }

            public (ItemType Type, int Level)[] RareItems { get; private set; }

            public (ItemType Type, int Level, int RequiredStars)[] Targets { get; private set; }

            public int MaxOrderPoints { get; }

            public Settings((ItemType Type, int MinLevel, int MaxLevel, int RewardLevel)[] items, 
                (ItemType Type, int Level)[] rareItems, 
                (ItemType Type, int Level, int RequiredStars)[] targets, 
                int maxOrderPoints = 3)
            {
                Items = items;
                RareItems = rareItems;
                Targets = targets;
                MaxOrderPoints = maxOrderPoints;
            }

            public void ClearRareItems() => RareItems = null;
        }
    }
}