using UnityEngine;
using Enums;
using Service;
using Gameplay.Counters;
using Gameplay.Tutorial;
using System;

namespace Gameplay.Field
{
    [RequireComponent(typeof(QuickClickTracking))]
    [RequireComponent(typeof(Item))]
    public class ItemCurrency : MonoBehaviour
    {
        private QuickClickTracking _quickClickTracking;
        private Item _item;

        private CurrencyType _type;

        public static event Action Collected;

        private void Awake()
        {
            _item = GetComponent<Item>();
            _quickClickTracking = GetComponent<QuickClickTracking>();
        }

        public void SetType(CurrencyType type) => _type = type;

        private void Update()
        {
            if (_quickClickTracking.QuickClicked && (TutorialSystem.TutorialDone || GetComponent<TutorialExtraTarget>() != null))
                GetReward();
        }

        private void GetReward()
        {
            var storage = GameStorage.Instance;
            var currencyAdder = storage.GetComponent<CurrencyAdder>();
            var reward = 0;
            if (_type == CurrencyType.Star) reward = storage.GetStarsRewardByItemLevel(_item.Stats.Level);
            else if (_type == CurrencyType.Brilliant) reward = storage.GetBrilliantsRewardByItemlevel(_item.Stats.Level);
            currencyAdder.Add(_type, reward, transform.position);
            _item.Remove();
            SoundManager.Instanse.Play(Sound.Brilliant, null);
            Collected?.Invoke();
        }
    }
}
