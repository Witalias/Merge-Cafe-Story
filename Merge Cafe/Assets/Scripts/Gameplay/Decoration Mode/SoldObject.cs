using UnityEngine;
using Service;
using Enums;

namespace Gameplay.DecorationMode
{
    public class SoldObject : MonoBehaviour
    {
        [SerializeField] private bool _startActive = false;
        [SerializeField] private Sprite _icon;
        [SerializeField] private int _cost;
        [SerializeField] private SoldObject[] _nextObjectForSell;

        public void Activate()
        {
            var buildingCanvas = GameObject.FindGameObjectWithTag(Tags.BuildingCanvas.ToString()).transform;
            var purchaseButton = Instantiate(GameStorage.Instanse.PurchaseButton, transform.position, Quaternion.identity, buildingCanvas).GetComponent<PurchaseButton>();
            purchaseButton.SetIcon(_icon);
            purchaseButton.SetCost(_cost);

            void SetObjectActive()
            {
                gameObject.SetActive(true);
                Instantiate(GameStorage.Instanse.Sequins, transform.position, Quaternion.identity, transform.parent);
                foreach (var nextSellObject in _nextObjectForSell)
                    nextSellObject.Activate();
            }

            purchaseButton.SetPurchaseAction(SetObjectActive);
        }

        private void Start()
        {
            if (_startActive)
                Activate();

            gameObject.SetActive(false);
        }
    }
}