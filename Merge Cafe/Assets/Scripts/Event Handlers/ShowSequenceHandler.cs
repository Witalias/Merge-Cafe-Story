using UnityEngine;
using UI;
using Gameplay.Field;
using Gameplay.Orders;
using Enums;
using Service;
using Gameplay.Tutorial;

namespace EventHandlers
{
    [RequireComponent(typeof(SequencePanel))]
    public class ShowSequenceHandler : MonoBehaviour
    {
        private SequencePanel _sequencePanel;

        private void Awake()
        {
            _sequencePanel = GetComponent<SequencePanel>();
        }

        private void OnEnable()
        {
            Item.CursorHoveredMovableItem += Show;
            Item.CursorHoveredNotMovableItem += ShowCombination;
            OrderPoint.CursorHoveredItemInOrder += Show;
            TutorialSystem.GetItemInSequence += _sequencePanel.GetItemInSequence;
        }

        private void OnDisable()
        {
            Item.CursorHoveredMovableItem -= Show;
            Item.CursorHoveredNotMovableItem -= ShowCombination;
            OrderPoint.CursorHoveredItemInOrder -= Show;
            TutorialSystem.GetItemInSequence -= _sequencePanel.GetItemInSequence;
        }

        private void Show(ItemType type, int level)
        {
            Show(type);
        }

        private void Show(ItemType type)
        {
            _sequencePanel.Hide();
            _sequencePanel.Show(type);
        }

        private void ShowCombination(ItemType type, int level)
        {
            _sequencePanel.Hide();
            _sequencePanel.ShowCombination(GameStorage.Instance.GetSecondItemTypeInCombination(type), type);
        }
    }
}
