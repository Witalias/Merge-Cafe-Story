using UnityEngine;
using Service;
using Enums;
using EventHandlers;
using Gameplay.Tutorial;

namespace Gameplay.DecorationMode
{
    public class SoldObject : MonoBehaviour, IStorable
    {
        private const string SOLD_OBJECT_CAN_BUY_KEY = "SOLD_OBJECT_CAN_BUY_";
        private const string SOLD_OBJECT_PURCHASED_KEY = "SOLD_OBJECT_PURCHASED_";

        [SerializeField] private bool _startActive = false;
        [SerializeField] private bool _hideable = false;
        [SerializeField] private Sprite _icon;
        [SerializeField] private int _cost;
        [SerializeField] private DialogActivationPoint _dialogActivationPoint;
        [SerializeField] private SoldObject[] _nextObjectsForSell;

        private bool _canBuy = false;
        private bool _purchased = false;

        public void Save()
        {
            //if (!TutorialSystem.TutorialDone)
            //    return;

            var code = GetID();
            PlayerPrefs.SetInt(SOLD_OBJECT_CAN_BUY_KEY + code, _canBuy ? 1 : 0);
            PlayerPrefs.SetInt(SOLD_OBJECT_PURCHASED_KEY + code, _purchased ? 1 : 0);
        }

        public void Load()
        {
            var code = GetID();
            var a = PlayerPrefs.GetInt(SOLD_OBJECT_PURCHASED_KEY + code, 0);
            if (PlayerPrefs.GetInt(SOLD_OBJECT_PURCHASED_KEY + code, 0) == 1)
                gameObject.SetActive(!_hideable);
            else
            {
                gameObject.SetActive(_hideable);
                if (_startActive || PlayerPrefs.GetInt(SOLD_OBJECT_CAN_BUY_KEY + code, 0) == 1)
                    Activate();
            }

        }

        public void Activate()
        {
            var buildingCanvas = GameObject.FindGameObjectWithTag(Tags.BuildingCanvas.ToString()).transform;
            var spawnPosition = new Vector3(transform.position.x, transform.position.y, 0f);
            var purchaseButton = Instantiate(GameStorage.Instance.PurchaseButton, transform.position, Quaternion.identity, buildingCanvas)
                .GetComponent<PurchaseButton>();
            purchaseButton.SetIcon(_icon);
            purchaseButton.SetCost(_cost);
            _canBuy = true;

            void SetObjectActive()
            {
                _canBuy = false;
                _purchased = true;
                gameObject.SetActive(!_hideable);
                Instantiate(GameStorage.Instance.Sequins, transform.position, Quaternion.identity, transform.parent);
                foreach (var nextSellObject in _nextObjectsForSell)
                    nextSellObject.Activate();
                if (_dialogActivationPoint != null)
                    _dialogActivationPoint.Activate();
                Save();
            }

            purchaseButton.SetPurchaseAction(SetObjectActive);
            Save();
        }

        private void Start()
        {
            if (GameStorage.Instance.LoadData && PlayerPrefs.HasKey(SOLD_OBJECT_PURCHASED_KEY + GetID()))
            {
                Load();
                return;
            }

            if (_startActive)
                Activate();

            gameObject.SetActive(_hideable);
        }

        private string GetID() => gameObject.name + transform.GetSiblingIndex();
    }
}