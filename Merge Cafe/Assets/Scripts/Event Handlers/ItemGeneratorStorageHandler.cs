using Gameplay.Counters;
using Gameplay.ItemGenerators;
using UnityEngine;

namespace EventHandlers
{
    [RequireComponent(typeof(ItemGeneratorStorage))]
    public class ItemGeneratorStorageHandler : MonoBehaviour
    {
        private ItemGeneratorStorage _itemGeneratorStorage;

        private void Awake()
        {
            _itemGeneratorStorage = GetComponent<ItemGeneratorStorage>();
        }

        private void OnEnable()
        {
            StarCounter.IsGenerator += _itemGeneratorStorage.IsGenerator;
            StarCounter.GeneratorExistsInGame += _itemGeneratorStorage.ExistsInGame;
            StarCounter.ActivateGenerator += _itemGeneratorStorage.Activate;
        }

        private void OnDisable()
        {
            StarCounter.IsGenerator -= _itemGeneratorStorage.IsGenerator;
            StarCounter.GeneratorExistsInGame -= _itemGeneratorStorage.ExistsInGame;
            StarCounter.ActivateGenerator -= _itemGeneratorStorage.Activate;
        }
    }
}
