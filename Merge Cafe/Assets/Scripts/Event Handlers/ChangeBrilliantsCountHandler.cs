using UnityEngine;
using Service;
using TMPro;
using Gameplay.Counters;

namespace EventHandlers
{
    public class ChangeBrilliantsCountHandler : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _value;

        private GameStorage _storage;

        private void Start()
        {
            _storage = GameStorage.Instanse;
            UpdateText();
        }

        private void OnEnable()
        {
            CurrencyAdder.BrilliantsChanged += Add;
            UpdateText();
        }

        private void OnDisable()
        {
            CurrencyAdder.BrilliantsChanged -= Add;
        }

        private void Add(int brilliants)
        {
            _storage.BrilliantsCount += brilliants;
            UpdateText();
            SoundManager.Instanse.Play(Enums.Sound.Brilliant, null);
        }

        private void UpdateText()
        {
            if (_value != null && _storage != null)
                _value.text = _storage.BrilliantsCount.ToString();
        }
    }
}