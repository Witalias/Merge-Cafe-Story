using UnityEngine;
using Service;
using Enums;

namespace Gameplay.DecorationMode
{
    public class SoldObject : MonoBehaviour
    {
        [SerializeField] private bool _startActive = false;
        [SerializeField] private bool _hideable = false;
        [SerializeField] private Sprite _icon;
        [SerializeField] private int _cost;
        [SerializeField] private DialogActivationPoint _dialogActivationPoint;
        [SerializeField] private SoldObject[] _nextObjectsForSell;

        public void Activate()
        {
            var buildingCanvas = GameObject.FindGameObjectWithTag(Tags.BuildingCanvas.ToString()).transform;
            var spawnPosition = new Vector3(transform.position.x, transform.position.y, 0f);
            var purchaseButton = Instantiate(GameStorage.Instanse.PurchaseButton, transform.position, Quaternion.identity, buildingCanvas).GetComponent<PurchaseButton>();
            purchaseButton.SetIcon(_icon);
            purchaseButton.SetCost(_cost);

            void SetObjectActive()
            {
                gameObject.SetActive(!_hideable);
                Instantiate(GameStorage.Instanse.Sequins, transform.position, Quaternion.identity, transform.parent);
                foreach (var nextSellObject in _nextObjectsForSell)
                    nextSellObject.Activate();
                if (_dialogActivationPoint != null)
                    _dialogActivationPoint.Activate();
            }

            purchaseButton.SetPurchaseAction(SetObjectActive);
        }

        private void Start()
        {
            if (_startActive)
                Activate();

            gameObject.SetActive(_hideable);
        }
    }
}