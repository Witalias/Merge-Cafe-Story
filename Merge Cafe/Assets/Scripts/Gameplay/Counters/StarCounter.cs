using UnityEngine;
using UnityEngine.UI;
using UI;
using TMPro;
using Service;

namespace Gameplay.Counters
{
    public class StarCounter : MonoBehaviour
    {
        [SerializeField] private UIBar _bar;
        [SerializeField] private TextMeshProUGUI _value;
        [SerializeField] private Image _nextPresent;
        [SerializeField] private float barSpeed = 1f;

        private GameStorage _storage;

        private float _currentBarValue = 0f;
        private int _needStarsToNextPresent = 10;
        private int _needPresentsToNextGameStage = 10;

        public void AddStars(int value)
        {
            _storage.StatsCount += value;
            UpdateValueText();
        }

        private void Start()
        {
            _storage = GameStorage.Instanse;
            UpdateValueText();
        }

        private void Update()
        {
            _currentBarValue = Mathf.Lerp(_currentBarValue, _storage.StatsCount, barSpeed * Time.deltaTime);
            _bar.SetValue(_currentBarValue / _needStarsToNextPresent * 100f);

            if (_bar.Filled)
                Next();
        }

        private void UpdateValueText()
        {
            _value.text = $"{_storage.StatsCount} / {_needStarsToNextPresent}";
        }

        private void Next()
        {
            --_needPresentsToNextGameStage;
            _storage.StatsCount -= _needStarsToNextPresent;
            UpdateValueText();
        }
    }

}