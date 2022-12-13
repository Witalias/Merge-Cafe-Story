using UnityEngine;
using Service;
using System.Collections.Generic;
using System.Linq;
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
        [SerializeField] private float _diffucultyMultiplierForRareOrder = 2;
        [SerializeField] private int _starsMultiplierForRareOrders = 5;
        [SerializeField] private int _brilliantsMultiplier = 10;
        [SerializeField] private int _brilliantsSpread = 7;
        [SerializeField] [Range(0f, 100f)] private float _extraRewardChance;
        [SerializeField] private int _extraRewardSpread = 5;
        [SerializeField] private Order.ExtraReward[] _extraRewards;

        private GameStorage _storage;

        private int _ordersCount = 1;
        private int _remainsToRareOrder = 1;
        private readonly Queue<ItemStorage> _rareItemsQueue = new();

        public void GenerateOrder(int id)
        {
            if (id > _ordersCount - 1)
            {
                Debug.LogWarning($"“екущее максимальное количество заказов: {_ordersCount}.");
                return;
            }

            var settings = GameStage.GetSettingsByStage(_storage.GameStage);
            if (settings == null)
                settings = GameStage.GetSettingsByStage(_storage.GameStage - 1);

            UpdateRareItemsQueue(settings);

            if (_remainsToRareOrder == 0 && _rareItemsQueue.Count > 0)
            {
                GenerateRareOrder(id);
                return;
            }
            GenerateOrdinaryOrder(id, settings);

            if (_rareItemsQueue.Count > 0)
                --_remainsToRareOrder;
        }

        public ItemStorage GetRandomOrderItem()
        {
            var orderPoints = GetOrderPoints();
            if (orderPoints.Length == 0)
                return null;
            return orderPoints[Random.Range(0, orderPoints.Length)];
        }

        public ItemStorage GetOrderItemMaxLevel()
        {
            var orderPoints = GetOrderPoints();
            if (orderPoints.Length == 0)
                return null;
            return orderPoints.OrderByDescending(point => point.Level).ToArray()[0];
        }

        private ItemStorage[] GetOrderPoints() => _orders.SelectMany(order => order.OrderPoints).ToArray();

        private void GenerateOrdinaryOrder(int id, GameStage.Settings settings)
        {
            var possibleItems = new List<(ItemType Type, int MinLevel, int MaxLevel, int RewardLevel)>(settings.Items);
            var pointsCount = Random.Range(1, settings.MaxOrderPoints + 1);
            var itemsToOrder = new ItemStorage[pointsCount];
            var starsReward = 0;
            var brilliantsReward = 0;
            var difficulty = 0;
            for (var i = 0; i < pointsCount; ++i)
            {
                var item = possibleItems[Random.Range(0, possibleItems.Count)];
                var randomLevel = Random.Range(item.MinLevel, item.MaxLevel + 1);
                itemsToOrder[i] = _storage.GetItem(item.Type, randomLevel);
                starsReward += _storage.GetStarsCountByItemLevel(randomLevel) + (item.RewardLevel - 1);
                brilliantsReward += GetBrilliantsReward(starsReward);
                difficulty += randomLevel;
                possibleItems.Remove(item);

                if (possibleItems.Count == 0)
                    break;
            }

            ItemStorage extraReward = null;
            if (Random.Range(0f, 100f) > _extraRewardChance)
                extraReward = GetRandomExtraReward(difficulty);

            _orders[id].Generate(itemsToOrder, starsReward, brilliantsReward, extraReward);
        }

        private void GenerateRareOrder(int id)
        {
            UpdateRemainToRareOrder();
            var rareItem = _rareItemsQueue.Dequeue();
            var stars = _storage.GetStarsCountByItemLevel(rareItem.Level) * _starsMultiplierForRareOrders;
            _orders[id].Generate(new[] { rareItem }, stars, GetBrilliantsReward(stars), GetRandomExtraReward((int)(rareItem.Level * _diffucultyMultiplierForRareOrder)));
        }

        public void AddNewOrder()
        {
            if (_ordersCount >= _maxOrdersCount)
            {
                Debug.LogWarning($"ћаксимальное количество заказов: {_maxOrdersCount}. Ѕольше никак не смогу добавить.");
                return;
            }
            ++_ordersCount;
            _orders[_ordersCount - 1].gameObject.SetActive(true);
            GenerateOrder(_ordersCount - 1);
        }

        private void Start()
        {
            _storage = GameStorage.Instanse;
            for (var i = 0; i < _orders.Length; ++i)
                _orders[i].SetID(i);
            UpdateRemainToRareOrder();
            GenerateOrder(0);
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

        private void UpdateRemainToRareOrder() => _remainsToRareOrder = Random.Range(_ordersCountBeforeRareOrder - _ordersCountBeforeRareOrderSpread,
                _ordersCountBeforeRareOrder + _ordersCountBeforeRareOrderSpread);

        private ItemStorage GetRandomExtraReward(int orderDifficulty)
        {
            var rewards = _extraRewards
                .Where(reward => orderDifficulty >= reward.MinOrderDifficulty)
                .OrderBy(reward => reward.MinOrderDifficulty)
                .TakeLast(_extraRewardSpread)
                .ToArray();

            if (rewards == null || rewards.Length == 0)
                return null;

            var randomReward = rewards[Random.Range(0, rewards.Length)];
            var item = GameStorage.Instanse.GetItem(randomReward.Type, randomReward.Level);
            return item;
        }
    }
}