using UnityEngine;
using Gameplay;
using Gameplay.Field;
using Gameplay.Counters;
using Gameplay.Orders;

namespace EventHandlers
{
    [RequireComponent(typeof(ItemQueue))]
    public class AddRewardToQueueHandler : MonoBehaviour
    {
        private ItemQueue _itemQueue;

        private void Awake()
        {
            _itemQueue = GetComponent<ItemQueue>();
        }

        private void OnEnable()
        {
            StarCounter.NoEmptyCellsAndRewardGetted += Add;
            Order.NoEmptyCellsAndRewardGetted += Add;
        }

        private void OnDisable()
        {
            StarCounter.NoEmptyCellsAndRewardGetted -= Add;
            Order.NoEmptyCellsAndRewardGetted -= Add;
        }

        private void Add(ItemStorage item)
        {
            _itemQueue.Add(item);
        }
    }
}
