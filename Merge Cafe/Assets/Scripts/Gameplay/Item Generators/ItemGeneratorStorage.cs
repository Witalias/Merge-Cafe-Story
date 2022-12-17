using UnityEngine;
using Gameplay.Field;
using System.Linq;
using Enums;
using Service;
using System.Collections.Generic;

namespace Gameplay.ItemGenerators
{
    public class ItemGeneratorStorage : MonoBehaviour, IStorable
    {
        private const string LAUNCHED_FIRST_TIME_KEY = "LAUNCHED_FIRST_TIME";

        [SerializeField] private Upgradable[] _upgradables;

        private ItemGenerator[] _generators;

        public void Save()
        {
            foreach (var upgradable in _upgradables)
                upgradable.Save();
        }

        public void Load()
        {
            foreach (var upgradable in _upgradables)
                upgradable.Load();
        }

        public Sprite[] GetProducedItemSprites(ItemType type)
        {
            var upgradables = GetUpgradables(type);

            if (upgradables.Length == 0)
                Debug.LogError($"Generator {type} wasn't found");

            var generator = upgradables[0].GetComponent<ItemGenerator>();
            var generatedItems = upgradables[0].GetComponent<ItemGenerator>().GeneratedItems;
            var sprites = new List<Sprite>();

            for (var i = 1; i <= generator.MaxItemsLevel; ++i)
            {
                foreach (var generatedItem in generatedItems)
                    sprites.Add(GameStorage.Instanse.GetItemSprite(generatedItem, i));
            }
            return sprites.ToArray();

        }

        public void SetActiveTimers(bool value)
        {
            foreach (var generator in _generators)
                generator.SetActiveTimer(value);
        }

        public bool ExistsInGame(ItemType type)
        {
            foreach (var upgradable in _upgradables)
            {
                if (upgradable.Type == type && upgradable.gameObject.activeSelf)
                    return true;
            }
            return false;
        }

        public void Activate(ItemType type)
        {
            var generatorsToActivate = _upgradables.Where(upgradable => upgradable.Type == type).ToArray();
            foreach (var generator in generatorsToActivate) generator.Activate();
        }

        public bool IsGenerator(ItemType type)
        {
            return GetUpgradables(type).Length != 0;
        }

        public bool IsMaxLevel(ItemType type, int level)
        {
            var generators = GetUpgradables(type);

            if (generators.Length == 0)
                return true;

            return level == generators[0].Level;
        }

        public int GetLevel(ItemType type)
        {
            var generators = GetUpgradables(type);

            if (generators.Length == 0)
                Debug.LogError($"Generator {type} wasn't found");

            return generators[0].Level;
        }

        private void Awake()
        {
            _generators = _upgradables
                .Where(upgradable => upgradable.GetComponent<ItemGenerator>() != null)
                .Select(upgradable => upgradable.GetComponent<ItemGenerator>())
                .ToArray();
        }

        private void Start()
        {
            if (GameStorage.Instanse.LoadData && PlayerPrefs.HasKey(LAUNCHED_FIRST_TIME_KEY))
                Load();
            PlayerPrefs.SetInt(LAUNCHED_FIRST_TIME_KEY, 1);
        }

        private Upgradable[] GetUpgradables(ItemType type)
        {
            var generators = _upgradables
                .Where(generator => generator.Type == type)
                .ToArray();

            return generators;
        }
    }
}