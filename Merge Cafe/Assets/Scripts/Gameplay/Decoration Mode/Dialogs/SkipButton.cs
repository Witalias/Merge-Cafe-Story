using UnityEngine;
using TMPro;
using System;

namespace Gameplay.DecorationMode.Dialogs
{
    public class SkipButton : MonoBehaviour
    {
        public static event Action SkipDialogButtonPressed;

        private void OnMouseDown()
        {
            SkipDialogButtonPressed?.Invoke();
        }
    }
}
