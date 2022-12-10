using Enums;
using System.Collections.Generic;

namespace Service
{
    public static class PresentsInfo
    {
        private static readonly Content[][] _presentContents;

        static PresentsInfo()
        {
            _presentContents = new[]
            {
                new[]
                {
                    new Content(ItemType.Star, 1, 2, 2),
                    new Content(ItemType.Brilliant, 1, 2, 3),
                    new Content(ItemType.Key, 1, 1, 1)
                },
                new[]
                {
                    new Content(ItemType.Star, 2, 3, 3),
                    new Content(ItemType.Brilliant, 2, 3, 5),
                    new Content(ItemType.Key, 1, 2, 2)
                },
                new[]
                {
                    new Content(ItemType.Star, 2, 4, 4),
                    new Content(ItemType.Brilliant, 2, 4, 7),
                    new Content(ItemType.Key, 1, 3, 3)
                },
            };
        }

        public static Content[] GetContent(int presentLevel)
        {
            return _presentContents[presentLevel - 1];
        }

        public class Content
        {
            public int Count { get; }
            public ItemType Type { get; }
            public int MinLevel { get; }
            public int MaxLevel { get; }

            public Content(ItemType type, int minLevel, int maxLevel, int count)
            {
                Type = type;
                MinLevel = minLevel;
                MaxLevel = maxLevel;
                Count = count;
            }
        }
    }
}