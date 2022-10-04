using UnityEngine;

namespace AnimationEngine
{
    [RequireComponent(typeof(Animator))]
    public class CursorHoveredAnimation : MonoBehaviour
    {
        private const string _zoomAnimatorBool = "Mouse Enter";

        [Header("Animator Parameters:\nMouse Enter (bool)")]

        private Animator _animator;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        private void OnMouseEnter()
        {
            _animator.SetBool(_zoomAnimatorBool, true);
        }

        private void OnMouseExit()
        {
            _animator.SetBool(_zoomAnimatorBool, false);
        }
    }
}
