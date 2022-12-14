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

            InfoWindowHandler.IsGenerator += _itemGeneratorStorage.IsGenerator;
            InfoWindowHandler.IsGeneratorMaxLevel += _itemGeneratorStorage.IsMaxLevel;
            InfoWindowHandler.GetGeneratorLevel += _itemGeneratorStorage.GetLevel;
            InfoWindowHandler.GetProducedItemSprites += _itemGeneratorStorage.GetProducedItemSprites;
        }

        private void OnDisable()
        {
            StarCounter.IsGenerator -= _itemGeneratorStorage.IsGenerator;
            StarCounter.GeneratorExistsInGame -= _itemGeneratorStorage.ExistsInGame;
            StarCounter.ActivateGenerator -= _itemGeneratorStorage.Activate;

            InfoWindowHandler.IsGenerator -= _itemGeneratorStorage.IsGenerator;
            InfoWindowHandler.IsGeneratorMaxLevel -= _itemGeneratorStorage.IsMaxLevel;
            InfoWindowHandler.GetGeneratorLevel -= _itemGeneratorStorage.GetLevel;
            InfoWindowHandler.GetProducedItemSprites -= _itemGeneratorStorage.GetProducedItemSprites;
        }
    }
}
