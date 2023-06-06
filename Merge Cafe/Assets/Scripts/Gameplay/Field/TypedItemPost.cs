using Enums;
using System;
using UnityEngine;

namespace Gameplay.Field
{
    [System.Serializable]
    public class TypedItemPost
    {
        public ItemType Type;
        public bool Throwable = true;
        public bool Movable = true;
        public bool Special = false;
        public ItemTypeSoundsPost[] Items;
    }
}