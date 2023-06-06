using Gameplay.Tutorial;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.Tutorial
{
    public class DisableObjectInTutorial : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private Toggle _toggle;
        [SerializeField] private GameObject _objectToInactive;

        private void OnEnable()
        {
            TutorialSystem.TutorialStarted += DisableButton;
            TutorialSystem.TutorialEnded += ActivateButton;
        }

        private void OnDisable()
        {
            TutorialSystem.TutorialStarted -= DisableButton;
            TutorialSystem.TutorialEnded -= ActivateButton;
        }

        private void ActivateButton()
        {
            if (_button != null)
                _button.interactable = true;
            if (_toggle != null)
                _toggle.interactable = true;
            if (_objectToInactive != null)
                _objectToInactive.SetActive(true);
        }

        private void DisableButton()
        {
            if (_button != null)
                _button.interactable = false;
            if (_toggle != null)
                _toggle.interactable = false;
            if (_objectToInactive != null)
                _objectToInactive.SetActive(false);
        }
    }
}
