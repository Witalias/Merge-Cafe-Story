using System;
using System.Collections.Generic;
using Enums;

namespace Gameplay.DecorationMode.Dialogs
{
    [Serializable]
    public class DialogPhrase
    {
        public CharacterType Character;
        public Emotion Emotion;
        public string Phrase;
    }
}
