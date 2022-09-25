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
            CurrencyAdder.StarsChanged += starCounter.AddStars;
        }

        private void OnDisable()
        {
            CurrencyAdder.StarsChanged -= starCounter.AddStars;
        }
    }

}