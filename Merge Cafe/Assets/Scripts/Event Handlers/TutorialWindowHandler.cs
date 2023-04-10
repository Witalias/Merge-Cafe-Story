using Gameplay.Tutorial;
using UnityEngine;

namespace EventHandlers
{
    [RequireComponent(typeof(TutorialWindow))]
    public class TutorialWindowHandler : MonoBehaviour
    {
        private TutorialWindow _tutorialWindow;

        private void Awake()
        {
            _tutorialWindow = GetComponent<TutorialWindow>();
        }

        private void OnEnable()
        {
            TutorialSystem.ShowTutorialWindow += _tutorialWindow.Show;
            TutorialSystem.HideTutorialWindow += _tutorialWindow.Hide;
            TutorialSystem.HideTutorialWindowWithDelay += _tutorialWindow.HideWithDelay;
            TutorialSystem.UpdateTutorialWindow += _tutorialWindow.UpdateText;
        }

        private void OnDisable()
        {
            TutorialSystem.ShowTutorialWindow -= _tutorialWindow.Show;
            TutorialSystem.HideTutorialWindow -= _tutorialWindow.Hide;
            TutorialSystem.HideTutorialWindowWithDelay -= _tutorialWindow.HideWithDelay;
            TutorialSystem.UpdateTutorialWindow -= _tutorialWindow.UpdateText;
        }
    }
}