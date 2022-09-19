using UnityEngine;
using UI;
using Gameplay.Field;
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
            Item.CursorHoveredOverItem += Show;
        }

        private void OnDisable()
        {
            Item.CursorHoveredOverItem -= Show;
        }

        private void Show(ItemType type)
        {
            _sequencePanel.Hide();
            _sequencePanel.Show(type);
        }
    }
}
