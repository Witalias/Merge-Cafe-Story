using UnityEngine;
using Gameplay.Field;
using System.Linq;
using Enums;

namespace Gameplay.ItemGenerators
{
    public class ItemGeneratorStorage : MonoBehaviour
    {
        [SerializeField] private Upgradable[] _upgradables;

        private ItemGenerator[] _generators;

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
            return GetGenerators(type).Length != 0;
        }

        public bool IsMaxLevel(ItemStorage item)
        {
            var generators = GetGenerators(item.Type);

            if (generators.Length == 0)
                return true;

            return item.Level == generators[0].Level;
        }

        private void Awake()
        {
            _generators = _upgradables
                .Where(upgradable => upgradable.GetComponent<ItemGenerator>() != null)
                .Select(upgradable => upgradable.GetComponent<ItemGenerator>())
                .ToArray();
        }

        private Upgradable[] GetGenerators(ItemType type)
        {
            var generators = _upgradables
                .Where(generator => generator.Type == type)
                .ToArray();

            return generators;
        }
    }
}