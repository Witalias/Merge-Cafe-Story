using UnityEngine;
using Gameplay.Orders;
using System.Collections;
using Gameplay.Counters;

namespace EventHandlers
{
    [RequireComponent(typeof(OrderManager))]
    public class ManageOrdersHandler : MonoBehaviour
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
        }

        private void OnDisable()
        {
            Order.OrderDone -= _orderManager.GenerateOrder;
            StarCounter.UpdateOrderCount -= _orderManager.AddNewOrder;
        }
    }
}