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
            _message.Show(Translation.GetMessageText(GameStorage.Instance.Language, MessageType.MaxLevel));
        }

        private void ShowNoEmptyCells()
        {
            _message.Show(Translation.GetMessageText(GameStorage.Instance.Language, MessageType.NoEmptyCells));
        }

        private void ShowCannotBeThrownAway()
        {
            _message.Show(Translation.GetMessageText(GameStorage.Instance.Language, MessageType.CannotBeThrownAway));
        }

        private void ShowDragItemToTrashCan()
        {
            _message.Show(Translation.GetMessageText(GameStorage.Instance.Language, MessageType.DragItemToTrashCan));
        }

        private void ShowUpgraded()
        {
            _message.Show(Translation.GetMessageText(GameStorage.Instance.Language, MessageType.Upgraded));
        }

        private void ShowWrongLevelForCombination()
        {
            _message.Show(Translation.GetMessageText(GameStorage.Instance.Language, MessageType.WrongLevelForCombinating));
        }

        private void ShowNotEnoughBrilliants()
        {
            _message.Show(Translation.GetMessageText(GameStorage.Instance.Language, MessageType.NotEnougthBrilliants));
        }

        private void ShowNoOrderPoints()
        {
            _message.Show(Translation.GetMessageText(GameStorage.Instance.Language, MessageType.NoOrderPoints));
        }
    }
}
