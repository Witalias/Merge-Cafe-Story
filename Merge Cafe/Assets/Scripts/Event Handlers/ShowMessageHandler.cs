using UnityEngine;
using UI;
using Gameplay.Field;
using Gameplay;
using Service;

namespace EventHandlers
{
    [RequireComponent(typeof(Message))]
    public class ShowMessageHandler : MonoBehaviour
    {
        private const string _maxLevelText = "Максимальный уровень";
        private const string _noEmptyCellsText = "Нет свободных ячеек";
        private const string _cannotBeThrownAwayText = "Этот предмет нельзя выбросить";
        private const string _dragItemToTrashCanText = "Перетащите ненужный предмет";

        private Message _message;

        private void Awake()
        {
            _message = GetComponent<Message>();
        }

        private void OnEnable()
        {
            Item.MergingItemsOfMaxLevelTried += ShowMaxLevel;
            Item.CannotBeThrownAway += ShowCannotBeThrownAway;
            GameStorage.NoEmptyCells += ShowNoEmptyCells;
            TrashCan.TrashCanClicked += ShowDragItemToTrashCan;
        }

        private void OnDisable()
        {
            Item.MergingItemsOfMaxLevelTried -= ShowMaxLevel;
            Item.CannotBeThrownAway -= ShowCannotBeThrownAway;
            GameStorage.NoEmptyCells -= ShowNoEmptyCells;
            TrashCan.TrashCanClicked -= ShowDragItemToTrashCan;
        }

        private void ShowMaxLevel()
        {
            _message.Show(_maxLevelText);
        }

        private void ShowNoEmptyCells()
        {
            _message.Show(_noEmptyCellsText);
        }

        private void ShowCannotBeThrownAway()
        {
            _message.Show(_cannotBeThrownAwayText);
        }

        private void ShowDragItemToTrashCan()
        {
            _message.Show(_dragItemToTrashCanText);
        }
    }
}
