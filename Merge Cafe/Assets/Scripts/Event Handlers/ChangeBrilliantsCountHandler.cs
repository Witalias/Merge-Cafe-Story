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
            Add(_storage.BrilliantsCount);
        }

        private void OnEnable()
        {
            CurrencyAdder.BrilliantsChanged += Add;
        }

        private void OnDisable()
        {
            CurrencyAdder.BrilliantsChanged -= Add;
        }

        private void Add(int brilliants)
        {
            _storage.BrilliantsCount += brilliants;
            _value.text = _storage.BrilliantsCount.ToString();
        }
    }
}