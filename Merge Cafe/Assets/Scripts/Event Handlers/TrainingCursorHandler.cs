using Gameplay.Tutorial;
using UnityEngine;

namespace EventHandlers
{
    [RequireComponent(typeof(TrainingCursor))]
    public class TrainingCursorHandler : MonoBehaviour
    {
        private TrainingCursor _cursor;

        private void Awake()
        {
            _cursor = GetComponent<TrainingCursor>();
        }

        private void OnEnable()
        {
            TutorialSystem.PlayClickAnimationCursor += _cursor.ClickAnimation;
            TutorialSystem.PlayDragAnimationCursor += _cursor.DragAnimation;
            TutorialSystem.StopAnimationCursor += _cursor.Kill;
            TutorialSystem.RotateCursor += _cursor.SetRotation;
        }

        private void OnDisable()
        {
            TutorialSystem.PlayClickAnimationCursor -= _cursor.ClickAnimation;
            TutorialSystem.PlayDragAnimationCursor -= _cursor.DragAnimation;
            TutorialSystem.StopAnimationCursor -= _cursor.Kill;
            TutorialSystem.RotateCursor -= _cursor.SetRotation;
        }
    }
}
