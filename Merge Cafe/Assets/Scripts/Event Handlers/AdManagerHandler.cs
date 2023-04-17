using Service;
using UI;
using UnityEngine;

namespace EventHandlers
{
    [RequireComponent(typeof(AdManager))]
    public class AdManagerHandler : MonoBehaviour
    {
        private AdManager _adManager;

        private void Awake()
        {
            _adManager = GetComponent<AdManager>();
        }

        private void OnEnable()
        {
            ScreenInfoWindow.AchivementRewardGetted += _adManager.ShowRewarded;
        }

        private void OnDisable()
        {
            ScreenInfoWindow.AchivementRewardGetted += _adManager.ShowRewarded;
        }
    }
}