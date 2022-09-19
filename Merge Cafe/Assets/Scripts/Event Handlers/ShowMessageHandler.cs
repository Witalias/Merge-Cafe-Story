using UnityEngine;
using UI;
using Gameplay.Field;

namespace EventHandlers
{
    [RequireComponent(typeof(Message))]
    public class ShowMessageHandler : MonoBehaviour
    {
        private const string _maxLevelText = "Максимальный уровень";

        private Message _message;

        private void Awake()
        {
            _message = GetComponent<Message>();
        }

        private void OnEnable()
        {
            Item.JoiningItemsOfMaxLevelTried += ShowMaxLevel;
        }

        private void OnDisable()
        {
            Item.JoiningItemsOfMaxLevelTried -= ShowMaxLevel;
        }

        private void ShowMaxLevel()
        {
            _message.Show(_maxLevelText);
        }
    }
}
