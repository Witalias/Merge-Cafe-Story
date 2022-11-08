using UnityEngine;
using Gameplay.DecorationMode.Dialogs;

namespace EventHandlers
{
    [RequireComponent(typeof(DialogCreator))]
    public class SkipDialogHandler : MonoBehaviour
    {
        private DialogCreator _dialogCreator;

        private void Awake()
        {
            _dialogCreator = GetComponent<DialogCreator>();
        }

        private void OnEnable()
        {
            SkipButton.SkipDialogButtonPressed += _dialogCreator.Skip;
        }

        private void OnDisable()
        {
            SkipButton.SkipDialogButtonPressed -= _dialogCreator.Skip;
        }
    }
}