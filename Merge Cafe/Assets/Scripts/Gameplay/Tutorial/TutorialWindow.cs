using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace Gameplay.Tutorial
{
    public class TutorialWindow : MonoBehaviour
    {
        [SerializeField] private GameObject _panel;
        [SerializeField] private TextMeshProUGUI _title;
        [SerializeField] private TextMeshProUGUI _mainText;

        private Coroutine _hideWithDelay;

        public static event Action HidedAfterDelay;

        public void Show(Vector2 position, string title, string mainText)
        {
            if (_hideWithDelay != null)
                StopCoroutine(_hideWithDelay);

            transform.position = position;
            _title.text = title;
            _mainText.text = mainText;
            StartCoroutine(Refresh());
        }

        public void Hide()
        {
            if (_hideWithDelay != null)
                StopCoroutine(_hideWithDelay);

            _panel.SetActive(false);
        }

        public void UpdateText(string title, string mainText)
        {
            _title.text = title;
            _mainText.text = mainText;
            if (_panel.activeSelf)
                StartCoroutine(Refresh());
        }

        public void HideWithDelay(float delay)
        {
            _hideWithDelay = StartCoroutine(HideWithDelayCoroutine(delay));
        }

        private IEnumerator HideWithDelayCoroutine(float delay)
        {
            yield return new WaitForSeconds(delay);
            Hide();
            HidedAfterDelay?.Invoke();
        }

        private IEnumerator Refresh()
        {
            _panel.SetActive(true);
            yield return new WaitForEndOfFrame();
            _panel.SetActive(false);
            _panel.SetActive(true);
        }
    }
}
