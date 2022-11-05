using System;
using Enums;

namespace Gameplay.DecorationMode.Dialogs
{
    [Serializable]
    public class DialogPhrase
    {
        public CharacterName Character;
        public Emotion Emotion;
        [UnityEngine.Multiline]
        public string Phrase;
    }
}
