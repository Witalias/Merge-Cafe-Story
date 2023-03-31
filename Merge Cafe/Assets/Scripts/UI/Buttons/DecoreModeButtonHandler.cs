using Gameplay.Tutorial;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Buttons
{
    public class DecoreModeButtonHandler : MonoBehaviour
    {
        [SerializeField] private Button _button;

        public static event Action Clicked;

        private void Awake()
        {
            _button.onClick.AddListener(OnClick);
        }

        private void OnEnable()
        {
            TutorialSystem.TutorialStarted += Disable;
            TutorialSystem.TutorialEnded += Activate;
            TutorialSystem.SetActiveDecoreModeButton += SetActive;
            TutorialSystem.GetDecoreModeButtonTransform += GetTransform;
        }

        private void OnDisable()
        {
            TutorialSystem.TutorialStarted -= Disable;
            TutorialSystem.TutorialEnded -= Activate;
            TutorialSystem.SetActiveDecoreModeButton -= SetActive;
            TutorialSystem.GetDecoreModeButtonTransform -= GetTransform;
        }

        private void Activate() => _button.interactable = true;

        private void Disable() => _button.interactable = false;

        private void SetActive(bool value) => _button.interactable = value;

        private Transform GetTransform() => transform;

        private void OnClick() => Clicked?.Invoke();
    }
}
