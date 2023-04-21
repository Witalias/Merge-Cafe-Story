using Gameplay.Tutorial;
using UnityEngine;

namespace Service
{
    public class AdManager : MonoBehaviour
    {
        [SerializeField] private bool _enabled;

        public void ShowRewarded()
        {
            if (!_enabled || !TutorialSystem.TutorialDone)
                return;
            //YandexSDK.instance.ShowRewarded("O");
        }

        public void ShowInterstitial()
        {
            if (!_enabled || !TutorialSystem.TutorialDone)
                return;
            //YandexSDK.instance.ShowInterstitial();
        }
    }
}