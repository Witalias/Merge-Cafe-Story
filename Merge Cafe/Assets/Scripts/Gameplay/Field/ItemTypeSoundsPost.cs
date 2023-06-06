using Enums;
using System;
using UnityEngine;

namespace Gameplay.Field
{
    [Serializable]
    public class ItemTypeSoundsPost
    {
        public Sprite Icon;
        public Sound TakeSound;
        public Sound PutSound;
        public bool ShakeAnimation;
    }
}