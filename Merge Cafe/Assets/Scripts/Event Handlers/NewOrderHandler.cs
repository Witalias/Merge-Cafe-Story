using UnityEngine;
using Gameplay.Orders;
using System.Collections;

[RequireComponent(typeof(OrderManager))]
public class NewOrderHandler : MonoBehaviour
{
    private OrderManager orderManager;

    private void Awake()
    {
        orderManager = GetComponent<OrderManager>();
    }

    private void OnEnable()
    {
        Order.OrderDone += orderManager.GenerateOrder;
    }

    private void OnDisable()
    {
        Order.OrderDone -= orderManager.GenerateOrder;
    }
}
