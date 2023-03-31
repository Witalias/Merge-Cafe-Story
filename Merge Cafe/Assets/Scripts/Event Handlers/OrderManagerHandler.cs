using UnityEngine;
using Gameplay.Orders;
using System.Collections;
using Gameplay.Counters;
using Gameplay.Field;
using Gameplay.Tutorial;

namespace EventHandlers
{
    [RequireComponent(typeof(OrderManager))]
    public class OrderManagerHandler : MonoBehaviour
    {
        private OrderManager _orderManager;

        private void Awake()
        {
            _orderManager = GetComponent<OrderManager>();
        }

        private void OnEnable()
        {
            Order.OrderDone += _orderManager.GenerateOrder;
            StarCounter.UpdateOrderCount += _orderManager.AddNewOrder;
            Box.GetRandomOrderItem += _orderManager.GetRandomOrderItem;
            Box.GetOrderItemMaxLevel += _orderManager.GetOrderItemMaxLevel;
            TutorialSystem.GenerateOrder += _orderManager.GenerateCustomOrder;
            TutorialSystem.GetOrderTransform += _orderManager.GetOrderTransform;
        }

        private void OnDisable()
        {
            Order.OrderDone -= _orderManager.GenerateOrder;
            StarCounter.UpdateOrderCount -= _orderManager.AddNewOrder;
            Box.GetRandomOrderItem -= _orderManager.GetRandomOrderItem;
            Box.GetOrderItemMaxLevel -= _orderManager.GetOrderItemMaxLevel;
            TutorialSystem.GenerateOrder -= _orderManager.GenerateCustomOrder;
            TutorialSystem.GetOrderTransform -= _orderManager.GetOrderTransform;
        }
    }
}