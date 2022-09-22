using Enums;

namespace Service
{
    public static class GameStage
    {
        private static readonly Settings[] storage;

        static GameStage()
        {
            storage = new[]
            {
                new Settings(new[]
                {
                    (ItemType.Tea, 1, 3),
                    (ItemType.Coffee, 1, 3),
                }, null, 3)
            };
        }

        public static Settings GetSettingsByStage(int value)
        {
            if (value > storage.Length)
                value = storage.Length;
            return storage[value - 1];
        }

        public class Settings
        {
            public (ItemType Type, int MinLevel, int MaxLevel)[] Items { get; }

            public (ItemType Type, int Level)[] RareItems { get; private set; }

            public int MaxOrderPoints { get; }

            public Settings((ItemType Type, int MinLevel, int MaxLevel)[] items, (ItemType Type, int Level)[] rareItems, int maxOrderPoints = 3)
            {
                Items = items;
                RareItems = rareItems;
                MaxOrderPoints = maxOrderPoints;
            }

            public void ClearRareItems() => RareItems = null;
        }
    }
}