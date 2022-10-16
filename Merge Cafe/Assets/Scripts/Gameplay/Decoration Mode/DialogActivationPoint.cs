using UnityEngine;
using Enums;
using Service;
using System;

namespace Gameplay.DecorationMode
{
    public class DialogActivationPoint : MonoBehaviour
    {
        [SerializeField] private DialogTitle _dialogType;
        [SerializeField] private int _cost;
        [SerializeField] private SoldObject[] _nextObjectsForSell;

        public static event Action<DialogTitle> DialogStarted;

        public void Activate()
        {
            var buildingCanvas = GameObject.FindGameObjectWithTag(Tags.BuildingCanvas.ToString()).transform;
            var purchaseButton = Instantiate(GameStorage.Instanse.PurchaseButton, transform.position, Quaternion.identity, buildingCanvas).GetComponent<PurchaseButton>();
            purchaseButton.SetIcon(GameStorage.Instanse.DialogWindow);
            purchaseButton.SetCost(_cost);

            void StartDialog()
            {
                DialogStarted?.Invoke(_dialogType);
                gameObject.SetActive(false);
                foreach (var nextSellObject in _nextObjectsForSell)
                    nextSellObject.Activate();
            }

            purchaseButton.SetPurchaseAction(StartDialog);
        }
    }
}