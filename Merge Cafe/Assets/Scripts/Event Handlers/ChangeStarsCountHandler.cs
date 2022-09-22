using UnityEngine;
using Gameplay.Counters;
using Gameplay.Orders;

namespace EventHandlers
{
    [RequireComponent(typeof(StarCounter))]
    public class ChangeStarsCountHandler : MonoBehaviour
    {
        private StarCounter starCounter;

        private void Awake()
        {
            starCounter = GetComponent<StarCounter>();
        }

        private void OnEnable()
        {
            Order.StarsReceived += starCounter.AddStars;
        }

        private void OnDisable()
        {
            Order.StarsReceived -= starCounter.AddStars;
        }
    }

}