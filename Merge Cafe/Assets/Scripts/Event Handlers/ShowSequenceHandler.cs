using UnityEngine;
using UI;
using Gameplay.Field;
using Gameplay.Orders;
using Enums;

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
            Item.CursorHoveredItem += Show;
            OrderPoint.CursorHoveredItemInOrder += Show;
        }

        private void OnDisable()
        {
            Item.CursorHoveredItem -= Show;
            OrderPoint.CursorHoveredItemInOrder -= Show;
        }

        private void Show(ItemType type)
        {
            _sequencePanel.Hide();
            _sequencePanel.Show(type);
        }
    }
}
