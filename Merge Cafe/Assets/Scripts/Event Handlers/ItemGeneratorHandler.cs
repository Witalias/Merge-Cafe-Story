using Gameplay.Counters;
using Gameplay.ItemGenerators;
using UnityEngine;

namespace EventHandlers
{
    [RequireComponent(typeof(ItemGenerator))]
    public class ItemGeneratorHandler : MonoBehaviour
    {
        private ItemGenerator _generator;

        private void Awake()
        {
            _generator = GetComponent<ItemGenerator>();
        }

        private void OnEnable()
        {
            StarCounter.NewStageReached += _generator.UpdateProducedItems;
        }

        private void OnDisable()
        {
            StarCounter.NewStageReached -= _generator.UpdateProducedItems;
        }
    }
}