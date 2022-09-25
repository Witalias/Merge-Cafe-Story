using UnityEngine;
using Service;
using System.Collections.Generic;
using Gameplay.Field;
using Enums;

namespace Gameplay.Orders
{
    public class OrderManager : MonoBehaviour
    {
        private const int _maxOrdersCount = 3;

        [SerializeField] private Order[] _orders;
        [SerializeField] private int _ordersCountBeforeRareOrder = 10;
        [SerializeField] private int _ordersCountBeforeRareOrderSpread = 5;
        [SerializeField] private float _delayBeforeNewOrder = 1f;
        [SerializeField] private int _starsMultiplierForRareOrders = 5;
        [SerializeField] private int _brilliantsMultiplier = 10;
        [SerializeField] private int _brilliantsSpread = 7;

        private int _ordersCount = 1;
        private int _remainsToRareOrder = 1;
        private readonly Queue<ItemStats> _rareItemsQueue = new Queue<ItemStats>();

        public float DelayBeforeNewOrder { get => _delayBeforeNewOrder; }

        private void Awake()
        {
            for (var i = 0; i < _orders.Length; ++i)
                _orders[i].SetID(i);
            UpdateRemainToRareOrder();
        }

        private void Start()
        {
            GenerateOrder(0);
        }

        public void GenerateOrder(int id)
        {
            if (id > _ordersCount - 1)
            {
                Debug.LogWarning($"Текущее максимальное количество заказов: {_ordersCount}. Я пока не могу добавить ещё один!");
                return;
            }

            var storage = GameStorage.Instanse;
            var settings = GameStage.GetSettingsByStage(storage.GameStage);
            if (settings == null)
                settings = GameStage.GetSettingsByStage(storage.GameStage - 1);

            UpdateRareItemsQueue(settings);

            if (_remainsToRareOrder == 0 && _rareItemsQueue.Count > 0)
            {
                UpdateRemainToRareOrder();
                var rareItem = _rareItemsQueue.Dequeue();
                var stars = storage.GetStarsCountByItemlevel(rareItem.Level) * _starsMultiplierForRareOrders;
                _orders[id].Generate(new[] { rareItem }, stars, GetBrilliantsReward(stars));
                return;
            }

            var possibleItems = new List<(ItemType Type, int MinLevel, int MaxLevel, int RewardLevel)>(settings.Items);
            var pointsCount = Random.Range(1, settings.MaxOrderPoints);
            var itemsToOrder = new ItemStats[pointsCount];
            var starsReward = 0;
            var brilliantsReward = 0;
            for (var i = 0; i < pointsCount; ++i)
            {
                var item = possibleItems[Random.Range(0, possibleItems.Count)];
                var randomLevel = Random.Range(item.MinLevel, item.MaxLevel + 1);
                itemsToOrder[i] = storage.GetItem(item.Type, randomLevel);
                starsReward += storage.GetStarsCountByItemlevel(randomLevel) + (item.RewardLevel - 1);
                brilliantsReward += GetBrilliantsReward(starsReward);
                possibleItems.Remove(item);

                if (possibleItems.Count == 0)
                    break;
            }
            _orders[id].Generate(itemsToOrder, starsReward, brilliantsReward);

            if (_rareItemsQueue.Count > 0)
                --_remainsToRareOrder;
        }

        private int GetBrilliantsReward(int starsReward)
        {
            var brilliantsBase = starsReward * _brilliantsMultiplier;
            var brilliantsSpread = brilliantsBase / _brilliantsSpread;
            return Random.Range(brilliantsBase - brilliantsSpread, brilliantsBase + brilliantsSpread);
        }

        private void UpdateRareItemsQueue(GameStage.Settings settings)
        {
            if (settings.RareItems != null && settings.RareItems.Length > 0)
            {
                foreach (var (type, level) in settings.RareItems)
                    _rareItemsQueue.Enqueue(GameStorage.Instanse.GetItem(type, level));
                settings.ClearRareItems();
            }
        }

        public void AddNewOrder()
        {
            if (_ordersCount >= _maxOrdersCount)
            {
                Debug.LogWarning($"Максимальное количество заказов: {_maxOrdersCount}. Больше никак не смогу добавить.");
                return;
            }
            ++_ordersCount;
            _orders[_ordersCount - 1].gameObject.SetActive(true);
            GenerateOrder(_ordersCount - 1);
        }

        private void UpdateRemainToRareOrder() => _remainsToRareOrder = Random.Range(_ordersCountBeforeRareOrder - _ordersCountBeforeRareOrderSpread,
                _ordersCountBeforeRareOrder + _ordersCountBeforeRareOrderSpread);
    }
}