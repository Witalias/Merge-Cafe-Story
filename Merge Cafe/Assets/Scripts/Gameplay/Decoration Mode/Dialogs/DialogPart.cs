using Enums;
using System;

namespace Gameplay.DecorationMode.Dialogs
{
    [Serializable]
    public class DialogPart
    {
        public DialogTitle Type;
        public DialogPhrase[] Phrases;
    }
}