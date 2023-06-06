using UnityEngine;
using Gameplay.Orders;
using System.Collections;
using Gameplay.Counters;
using Gameplay.Field;
using Gameplay.Tutorial;

namespace EventHandlers
{
    [RequireComponent(typeof(Order))]
    public class OrderHandler : MonoBehaviour
    {
        private Order _order;

        private void Awake()
        {
            _order = GetComponent<Order>();
        }
        private void OnEnable()
        {
            OrderManager.DoublerStatusChanged += _order.ChangeRewardCoefficent;
        }

        private void OnDisable()
        {
            OrderManager.DoublerStatusChanged -= _order.ChangeRewardCoefficent;
        }
    }


}