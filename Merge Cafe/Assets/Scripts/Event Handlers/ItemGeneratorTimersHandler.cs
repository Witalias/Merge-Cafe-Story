using UnityEngine;
using Gameplay.ItemGenerators;

namespace EventHandlers
{
    [RequireComponent(typeof(ItemGeneratorStorage))]
    public class ItemGeneratorTimersHandler : MonoBehaviour
    {
        private ItemGeneratorStorage _generatorStorage;

        private void Awake()
        {
            _generatorStorage = GetComponent<ItemGeneratorStorage>();
        }
    }
}
