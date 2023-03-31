using Gameplay.DecorationMode;
using Gameplay.DecorationMode.Dialogs;
using Gameplay.Field;
using Gameplay.ItemGenerators;
using Gameplay.Orders;
using Gameplay.Tutorial;
using UI;
using UI.Buttons;
using UnityEngine;

namespace EventHandlers
{
    [RequireComponent(typeof(TutorialSystem))]
    public class TutorialSystemHandler : MonoBehaviour
    {
        private TutorialSystem _tutorialSystem;

        private void Awake()
        {
            _tutorialSystem = GetComponent<TutorialSystem>();
        }

        private void OnEnable()
        {
            ItemGenerator.GeneratorClicked += _tutorialSystem.OnGeneratorClicked;
            ItemGenerator.ItemProduced += _tutorialSystem.OnItemProduced;
            Item.ItemCaptured += _tutorialSystem.OnItemCaptured;
            Order.OrderDone += _tutorialSystem.OnOrderCompleted;
            ItemInSequence.PresentGetted += _tutorialSystem.OnSequencePresentGetted;
            Item.ItemsMerged += _tutorialSystem.OnMerged;
            ItemCurrency.Collected += _tutorialSystem.OnCurrencyCollected;
            TrashCan.Throwed += _tutorialSystem.OnThrowed;
            DecoreModeButtonHandler.Clicked += _tutorialSystem.OnClickedDecorateButton;
            PurchaseButton.Purchased += _tutorialSystem.OnPurchased;
            DialogCreator.Ended += _tutorialSystem.OnDialogEnded;
            CheckingSameItems.CheckTutorialItem += _tutorialSystem.CheckTutorialItem;
            Item.ItemsCombinated += _tutorialSystem.OnItemsCombinated;
            Upgradable.Upgraded += _tutorialSystem.OnGeneratorUpgraded;
            ItemGenerator.Speeded += _tutorialSystem.OnGeneratorSpeeded;
        }

        private void OnDisable()
        {
            ItemGenerator.GeneratorClicked -= _tutorialSystem.OnGeneratorClicked;
            ItemGenerator.ItemProduced -= _tutorialSystem.OnItemProduced;
            Item.ItemCaptured -= _tutorialSystem.OnItemCaptured;
            Order.OrderDone -= _tutorialSystem.OnOrderCompleted;
            ItemInSequence.PresentGetted -= _tutorialSystem.OnSequencePresentGetted;
            Item.ItemsMerged -= _tutorialSystem.OnMerged;
            ItemCurrency.Collected -= _tutorialSystem.OnCurrencyCollected;
            TrashCan.Throwed -= _tutorialSystem.OnThrowed;
            DecoreModeButtonHandler.Clicked -= _tutorialSystem.OnClickedDecorateButton;
            PurchaseButton.Purchased -= _tutorialSystem.OnPurchased;
            DialogCreator.Ended -= _tutorialSystem.OnDialogEnded;
            CheckingSameItems.CheckTutorialItem -= _tutorialSystem.CheckTutorialItem;
            Item.ItemsCombinated -= _tutorialSystem.OnItemsCombinated;
            Upgradable.Upgraded -= _tutorialSystem.OnGeneratorUpgraded;
            ItemGenerator.Speeded -= _tutorialSystem.OnGeneratorSpeeded;
        }
    }
}