using UnityEngine;
using Service;
using TMPro;
using Gameplay.Counters;
using Gameplay.DecorationMode;

namespace EventHandlers
{
    public class ChangeBrilliantsCountHandler : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _value;
        [SerializeField] private GameObject _purchaseCanvas;
        [SerializeField] private GameObject _canBuyIcon;

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
            CheckSum();
        }

        private void OnDisable()
        {
            CurrencyAdder.BrilliantsChanged -= Add;
        }

        private void Add(int brilliants)
        {
            _storage.BrilliantsCount += brilliants;
            UpdateText();
            CheckSum();
        }

        private void CheckSum()
        {
            var purchases = _purchaseCanvas.GetComponentsInChildren<PurchaseButton>();
            foreach (var purchase in purchases)
            {
                var cost = purchase.ShowCost();
                if (cost <= _storage.BrilliantsCount)
                {
                    _canBuyIcon.SetActive(true);
                    return;
                }
            }
            _canBuyIcon.SetActive(false);
        }

        private void UpdateText()
        {
            if (_value != null && _storage != null)
                _value.text = _storage.BrilliantsCount.ToString();
        }
    }
}