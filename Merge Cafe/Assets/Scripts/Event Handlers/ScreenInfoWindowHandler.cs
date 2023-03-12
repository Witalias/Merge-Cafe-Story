using Gameplay.Counters;
using Gameplay.Field;
using Gameplay.ItemGenerators;
using UI;
using UnityEngine;

namespace EventHandlers
{
    [RequireComponent(typeof(ScreenInfoWindow))]
    public class ScreenInfoWindowHandler : MonoBehaviour
    {
        private ScreenInfoWindow _infoWindow;

        private void Awake()
        {
            _infoWindow = GetComponent<ScreenInfoWindow>();
        }

        private void OnEnable()
        {
            ItemStorage.NewItem += _infoWindow.Show;
            StarCounter.NewGlobalLevel += _infoWindow.Show;
            ItemGeneratorStorage.NewGenerator += _infoWindow.Show;
            Upgradable.ShowCongratulation += _infoWindow.ShowUpgradedGenerator;
        }

        private void OnDisable()
        {
            ItemStorage.NewItem -= _infoWindow.Show;
            StarCounter.NewGlobalLevel -= _infoWindow.Show;
            ItemGeneratorStorage.NewGenerator -= _infoWindow.Show;
            Upgradable.ShowCongratulation -= _infoWindow.ShowUpgradedGenerator;
        }
    }
}