using UnityEngine;
using Gameplay.Field;
using System.Linq;
using Enums;

namespace Gameplay.ItemGenerators
{
    public class ItemGeneratorStorage : MonoBehaviour
    {
        [SerializeField] private Upgradable[] _upgradables;

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

        private Upgradable[] GetGenerators(ItemType type)
        {
            var generators = _upgradables
                .Where(generator => generator.Type == type)
                .ToArray();

            return generators;
        }
    }
}