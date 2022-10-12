using UnityEngine;
using UI;
using Gameplay.Field;
using Gameplay.ItemGenerators;
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
        private const string _upgradedText = "УЛУЧШЕНО";
        private const string _wrongLevelForCombinating = "Неподходящий уровень предмета";

        private Message _message;

        private void Awake()
        {
            _message = GetComponent<Message>();
        }

        private void OnEnable()
        {
            Item.MergingItemsOfMaxLevelTried += ShowMaxLevel;
            Item.CannotBeThrownAway += ShowCannotBeThrownAway;
            Item.WrongLevelForCombinating += ShowWrongLevelForCombination;
            GameStorage.NoEmptyCells += ShowNoEmptyCells;
            TrashCan.TrashCanClicked += ShowDragItemToTrashCan;
            Upgradable.Upgraded += ShowUpgraded;
        }

        private void OnDisable()
        {
            Item.MergingItemsOfMaxLevelTried -= ShowMaxLevel;
            Item.CannotBeThrownAway -= ShowCannotBeThrownAway;
            Item.WrongLevelForCombinating -= ShowWrongLevelForCombination;
            GameStorage.NoEmptyCells -= ShowNoEmptyCells;
            TrashCan.TrashCanClicked -= ShowDragItemToTrashCan;
            Upgradable.Upgraded -= ShowUpgraded;
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

        private void ShowUpgraded()
        {
            _message.Show(_upgradedText);
        }

        private void ShowWrongLevelForCombination()
        {
            _message.Show(_wrongLevelForCombinating);
        }
    }
}
