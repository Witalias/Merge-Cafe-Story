using Gameplay.Counters;
using Gameplay.ItemGenerators;
using Gameplay.Tutorial;
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

            TutorialSystem.GetGenerator += _itemGeneratorStorage.GetGenerator;
            TutorialSystem.SetActiveGeneratorTimers += _itemGeneratorStorage.SetActiveTimers;
            TutorialSystem.SetGeneratorLevel += _itemGeneratorStorage.SetLevel;
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

            TutorialSystem.GetGenerator -= _itemGeneratorStorage.GetGenerator;
            TutorialSystem.SetActiveGeneratorTimers -= _itemGeneratorStorage.SetActiveTimers;
            TutorialSystem.SetGeneratorLevel -= _itemGeneratorStorage.SetLevel;
        }
    }
}
