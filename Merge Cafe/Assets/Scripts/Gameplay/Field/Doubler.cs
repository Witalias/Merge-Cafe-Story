using Service;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Field
{
    [RequireComponent(typeof(Item))]
    [RequireComponent(typeof(QuickClickTracking))]
    public class Doubler : MonoBehaviour
    {
        private Item _item;
        private QuickClickTracking _quickClickTracking;

        public static event System.Action<int> Activated;

        private readonly Dictionary<int, int> _ordersToDoubleDependingOnLevel = new Dictionary<int, int> ()
        {
            { 1, 1 },
            { 2, 4 },
            { 3, 9 }
        };

        private void Awake()
        {
            _item = GetComponent<Item>();
            _quickClickTracking = GetComponent<QuickClickTracking>();
        }

        void Update()
        {
            if (_quickClickTracking.QuickClicked)
            {
                Activated?.Invoke(_ordersToDoubleDependingOnLevel[_item.Stats.Level]);
                SoundManager.Instanse.Play(Enums.Sound.SpeedUp, null);
                Destroy(gameObject);
            }
        }
    }
}
