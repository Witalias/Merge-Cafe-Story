using Gameplay.Tutorial;
using UnityEngine;
using YG;

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
            YandexGame.RewVideoShow(0);
        }

        public void ShowInterstitial()
        {
            if (!_enabled || !TutorialSystem.TutorialDone)
                return;
            //YandexSDK.instance.ShowInterstitial();
            YandexGame.FullscreenShow();
        }
    }
}