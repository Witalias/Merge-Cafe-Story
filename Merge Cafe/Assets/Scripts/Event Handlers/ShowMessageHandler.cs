using UnityEngine;
using UI;
using Gameplay.Field;
using Service;

namespace EventHandlers
{
    [RequireComponent(typeof(Message))]
    public class ShowMessageHandler : MonoBehaviour
    {
        private const string _maxLevelText = "Максимальный уровень";
        private const string _noEmptyCellsText = "Нет свободных ячеек";

        private Message _message;

        private void Awake()
        {
            _message = GetComponent<Message>();
        }

        private void OnEnable()
        {
            Item.JoiningItemsOfMaxLevelTried += ShowMaxLevel;
            GameStorage.NoEmptyCells += ShowNoEmptyCells;
        }

        private void OnDisable()
        {
            Item.JoiningItemsOfMaxLevelTried -= ShowMaxLevel;
            GameStorage.NoEmptyCells -= ShowNoEmptyCells;
        }

        private void ShowMaxLevel()
        {
            _message.Show(_maxLevelText);
        }

        private void ShowNoEmptyCells()
        {
            _message.Show(_noEmptyCellsText);
        }
    }
}
