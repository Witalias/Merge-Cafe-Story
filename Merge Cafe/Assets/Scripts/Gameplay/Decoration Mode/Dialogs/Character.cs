using Enums;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.DecorationMode.Dialogs
{
    [Serializable]
    public class Character
    {
        public CharacterName Type;
        public string Name;
        public bool LeftSide = false;
        public TypedEmotion[] Emotions;

        private readonly Dictionary<Emotion, Sprite> _emotionDict = new Dictionary<Emotion, Sprite>();

        public Sprite GetEmotion(Emotion type) => _emotionDict[type];

        public void Initialize()
        {
            if (_emotionDict.Count > 0)
                return;

            foreach (var emotion in Emotions)
                _emotionDict.Add(emotion.Type, emotion.Face);
        }
    }
}
