using Gameplay.Orders;
using UnityEngine;
using Service;
using TMPro;

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
            Order.BrilliantsReceived += Add;
        }

        private void OnDisable()
        {
            Order.BrilliantsReceived -= Add;
        }

        private void Add(int brilliants)
        {
            _storage.BrilliantsCount += brilliants;
            _value.text = _storage.BrilliantsCount.ToString();
        }
    }
}