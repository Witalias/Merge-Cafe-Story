using UnityEngine;
using Service;
using TMPro;
using Gameplay.Counters;
using Gameplay.DecorationMode;

namespace EventHandlers
{
    public class ChangeBrilliantsCountHandler : MonoBehaviour, IStorable
    {
        private const string CAN_BUY_DECOR_KEY = "CAN_BUY_DECOR";

        [SerializeField] private TextMeshProUGUI _value;
        [SerializeField] private GameObject _purchaseCanvas;
        [SerializeField] private GameObject _canBuyIcon;

        private GameStorage _storage;

        public void Save()
        {
            PlayerPrefs.SetInt(CAN_BUY_DECOR_KEY, _canBuyIcon.activeSelf ? 1 : 0);
        }

        public void Load()
        {
            _canBuyIcon.SetActive(PlayerPrefs.GetInt(CAN_BUY_DECOR_KEY, 0) == 1);
        }

        public void RemoveNotificationIcon() => _canBuyIcon.SetActive(false);

        private void Start()
        {
            _storage = GameStorage.Instance;
            UpdateText();

            if (_storage.LoadData)
                Load();

            //if (!PlayerPrefs.HasKey(CAN_BUY_DECOR_KEY))
            //    _canBuyIcon.SetActive(true);
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

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.B) && GameStorage.Instance.Cheats)
                Add(1000);
        }

        private void Add(int brilliants)
        {
            _storage.BrilliantsCount += brilliants;
            SoundManager.Instanse.Play(Enums.Sound.Brilliant, null);
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
            
        }

        private void UpdateText()
        {
            if (_value != null && _storage != null)
                _value.text = _storage.BrilliantsCount.ToString();
        }
    }
}