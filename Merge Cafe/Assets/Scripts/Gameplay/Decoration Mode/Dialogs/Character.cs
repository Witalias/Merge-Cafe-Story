using Enums;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.DecorationMode.Dialogs
{
    [Serializable]
    public class Character
    {
        public CharacterType Type;
        public Name[] Names;
        public bool LeftSide = false;
        public TypedEmotion[] Emotions;

        private readonly Dictionary<Emotion, Sprite> _emotionDict = new();
        private readonly Dictionary<Language, string> _nameDict = new();

        public Sprite GetEmotion(Emotion type) => _emotionDict[type];

        public string GetName(Language language) => _nameDict[language];

        public void Initialize()
        {
            if (_emotionDict.Count > 0)
                return;

            foreach (var emotion in Emotions)
                _emotionDict.Add(emotion.Type, emotion.Face);

            foreach (var name in Names)
                _nameDict.Add(name.Language, name.Text);
        }

        [Serializable]
        public class Name
        {
            public Language Language;
            public string Text;
        }
    }
}
