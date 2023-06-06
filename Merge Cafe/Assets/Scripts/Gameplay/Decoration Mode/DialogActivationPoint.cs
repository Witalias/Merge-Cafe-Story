using UnityEngine;
using Enums;
using Service;
using System;
using EventHandlers;

namespace Gameplay.DecorationMode
{
    public class DialogActivationPoint : MonoBehaviour, IStorable
    {
        private const string DIALOG_POINT_CAN_ACTIVATE_KEY = "DIALOG_POINT_CAN_ACTIVATE";

        [SerializeField] private DialogTitle _dialogType;
        [SerializeField] private int _cost;
        [SerializeField] private SoldObject[] _nextObjectsForSell;

        private bool _canActivate = false;

        public static event Action<DialogTitle> DialogStarted;

        public void Save()
        {
            PlayerPrefs.SetInt(DIALOG_POINT_CAN_ACTIVATE_KEY + GetID(), _canActivate? 1 : 0);
        }

        public void Load()
        {
            if (PlayerPrefs.GetInt(DIALOG_POINT_CAN_ACTIVATE_KEY + GetID(), 0) == 1)
                Activate();
        }

        public void Activate()
        {
            var buildingCanvas = GameObject.FindGameObjectWithTag(Tags.BuildingCanvas.ToString()).transform;
            var purchaseButton = Instantiate(GameStorage.Instance.PurchaseButton, transform.position, Quaternion.identity, buildingCanvas).GetComponent<PurchaseButton>();
            purchaseButton.SetIcon(GameStorage.Instance.DialogWindow);
            purchaseButton.SetCost(_cost);
            _canActivate = true;

            void StartDialog()
            {
                _canActivate = false;
                gameObject.SetActive(false);
                foreach (var nextSellObject in _nextObjectsForSell)
                    nextSellObject.Activate();
                DialogStarted?.Invoke(_dialogType);
                Save();
            }

            purchaseButton.SetPurchaseAction(StartDialog);
            Save();
        }

        private void Start()
        {
            if (GameStorage.Instance.LoadData)
                Load();
        }

        private string GetID() => gameObject.name + transform.GetSiblingIndex();
    }
}