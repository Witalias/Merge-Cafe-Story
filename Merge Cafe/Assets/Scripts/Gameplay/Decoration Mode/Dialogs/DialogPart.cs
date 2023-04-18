using Enums;
using System;
using System.Collections.Generic;

namespace Gameplay.DecorationMode.Dialogs
{
    [Serializable]
    public class DialogPart
    {
        public DialogTitle Type;
        public TranslatedPart[] Parts;

        private readonly Dictionary<Language, DialogPhrase[]> _phraseDict = new();

        public int GetCount(Language language)
        {
            if (_phraseDict.Count == 0)
                Initialize();
            return _phraseDict[language].Length;
        }

        public DialogPhrase GetPhrase(Language language, int index)
        {
            if (_phraseDict.Count == 0)
                Initialize();

            if (_phraseDict.ContainsKey(language))
                return _phraseDict[language][index];
            else return _phraseDict[Language.English][index];
        }

        private void Initialize()
        {
            foreach (var part in Parts)
                _phraseDict.Add(part.Language, part.Phrases);
        }

        [Serializable]
        public class TranslatedPart
        {
            public Language Language;
            public DialogPhrase[] Phrases;
        }
    }
}