using UnityEngine;
using TMPro;

namespace UI
{
    [RequireComponent(typeof(Animator))]
    public class Message : MonoBehaviour
    {
        private const string _showAnimatorTrigger = "Show";

        [SerializeField] private TextMeshProUGUI _text;

        private Animator _animator;
        private Camera _mainCamera;

        public void Show(string message)
        {
            var mousePosition = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
            transform.position = Vector3.Lerp(Vector3.zero, new Vector3(mousePosition.x, mousePosition.y, 0f), 0.7f);
            _text.text = message;
            _animator.SetTrigger(_showAnimatorTrigger);
        }

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _mainCamera = Camera.main;
        }

        private void OnEnable()
        {
            transform.position = new Vector2(-5000, 0);
        }
    }
}
