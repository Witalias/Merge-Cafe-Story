using UnityEngine;
using UI;
using Gameplay.Field;
using Gameplay.Orders;
using Enums;
using Service;

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
        }

        private void OnDisable()
        {
            Item.CursorHoveredMovableItem -= Show;
            Item.CursorHoveredNotMovableItem -= ShowCombination;
            OrderPoint.CursorHoveredItemInOrder -= Show;
        }

        private void Show(ItemType type)
        {
            _sequencePanel.Hide();
            _sequencePanel.Show(type);
        }

        private void ShowCombination(ItemType type)
        {
            _sequencePanel.Hide();
            _sequencePanel.ShowCombination(GameStorage.Instanse.GetSecondItemTypeInCombination(type), type);
        }
    }
}
