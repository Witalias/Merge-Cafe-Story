using UnityEngine;
using UI;
using Gameplay.Field;
using Gameplay.ItemGenerators;
using Service;
using Gameplay.DecorationMode;

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
        private const string _wrongLevelForCombinatingText = "Неподходящий уровень предмета";
        private const string _notEnougthBrilliantsText = "Не хватает бриллиантов";
        private const string _noOrderPointsText = "Нет доступных заказов";

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
            PurchaseButton.NotEnoughBrilliants += ShowNotEnoughBrilliants;
            Box.NoOrderPoints += ShowNoOrderPoints;
        }

        private void OnDisable()
        {
            Item.MergingItemsOfMaxLevelTried -= ShowMaxLevel;
            Item.CannotBeThrownAway -= ShowCannotBeThrownAway;
            Item.WrongLevelForCombinating -= ShowWrongLevelForCombination;
            GameStorage.NoEmptyCells -= ShowNoEmptyCells;
            TrashCan.TrashCanClicked -= ShowDragItemToTrashCan;
            Upgradable.Upgraded -= ShowUpgraded;
            PurchaseButton.NotEnoughBrilliants -= ShowNotEnoughBrilliants;
            Box.NoOrderPoints -= ShowNoOrderPoints;

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
            _message.Show(_wrongLevelForCombinatingText);
        }

        private void ShowNotEnoughBrilliants()
        {
            _message.Show(_notEnougthBrilliantsText);
        }

        private void ShowNoOrderPoints()
        {
            _message.Show(_noOrderPointsText);
        }
    }
}
