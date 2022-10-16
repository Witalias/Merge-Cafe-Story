using UnityEngine;
using Gameplay.DecorationMode.Dialogs;
using Gameplay.DecorationMode;

namespace EventHandlers
{
    [RequireComponent(typeof(DialogCreator))]
    public class ActivateDialogHandler : MonoBehaviour
    {
        private DialogCreator _dialogCreator;

        private void Awake()
        {
            _dialogCreator = GetComponent<DialogCreator>();
        }

        private void OnEnable()
        {
            DialogActivationPoint.DialogStarted += _dialogCreator.Begin;
        }

        private void OnDisable()
        {
            DialogActivationPoint.DialogStarted -= _dialogCreator.Begin;
        }
    }
}