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
                new Settings(new[] // 1
                {
                    (ItemType.Tea, 1, 3, 1),
                }, new[]
                {
                    (ItemType.Tea, 4),
                }, new[]
                {
                    (ItemType.Teapot, 1, 7),
                    (ItemType.Brilliant, 1, 4),
                    (ItemType.Key, 1, 5),
                }, 1),

                new Settings(new[] // 2
                {
                    (ItemType.Tea, 1, 3, 1),
                    (ItemType.Coffee, 1, 3, 2),
                }, new[]
                {
                    (ItemType.Coffee, 4),
                }, new[]
                {
                    (ItemType.TrashCan, 1, 9),
                    (ItemType.Teapot, 1, 7),
                    (ItemType.Brilliant, 1, 4),
                    (ItemType.Brilliant, 2, 6),
                    (ItemType.Key, 1, 5),
                }, 1),

                new Settings(new[] // 3
                {
                    (ItemType.Tea, 1, 4, 1),
                    (ItemType.Coffee, 1, 3, 2),
                }, new[]
                {
                    (ItemType.Tea, 5),
                }, new[]
                {
                    (ItemType.Oven, 1, 20),
                    (ItemType.Teapot, 1, 7),
                    (ItemType.TrashCan, 1, 9),
                    (ItemType.Present, 1, 10),
                    (ItemType.Brilliant, 1, 5),
                    (ItemType.Key, 1, 6),
                    (ItemType.Key, 1, 6),
                    (ItemType.Brilliant, 2, 7),
                }, 1),

                new Settings(new[] // 4
                {
                    (ItemType.Tea, 1, 4, 1),
                    (ItemType.Coffee, 1, 4, 2),
                    (ItemType.BakeryProduct, 1, 3, 3),
                }, new[]
                {
                    (ItemType.Coffee, 5),
                    (ItemType.BakeryProduct, 4),
                }, new[]
                {
                    (ItemType.Oven, 1, 10),
                    (ItemType.Teapot, 1, 7),
                    (ItemType.Brilliant, 2, 8),
                    (ItemType.Brilliant, 3, 10),
                }, 2)
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

            public Settings(
                (ItemType Type, int MinLevel, int MaxLevel, int RewardLevel)[] items, 
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