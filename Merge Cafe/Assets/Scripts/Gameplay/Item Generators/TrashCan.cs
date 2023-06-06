using UnityEngine;
using UnityEngine.UI;
using Gameplay.Counters;
using Gameplay.Field;
using Service;
using Enums;
using System;

namespace Gameplay.ItemGenerators
{
    [RequireComponent(typeof(Upgradable))]
    public class TrashCan : MonoBehaviour
    {
        [SerializeField] private float[] _brilliantsMultiplierForLevels;

        private CurrencyAdder _currencyAdder;
        private Upgradable _upgradable;

        private int _initialLevel;

        public static event Action TrashCanClicked;
        public static event Action Throwed;

        public void Throw(int itemLevel)
        {
            if (_upgradable.Level <= _initialLevel)
            {
                SoundManager.Instanse.Play(Sound.TrashCan, null);
            }
            else
            {
                var brilliantsCount = (int)(itemLevel * _brilliantsMultiplierForLevels[_upgradable.Level - _initialLevel - 1]);
                _currencyAdder.Add(CurrencyType.Brilliant, brilliantsCount, transform.position);
                SoundManager.Instanse.Play(Sound.Sell, null);
            }
            Throwed?.Invoke();
        }

        private void Awake()
        {
            _upgradable = GetComponent<Upgradable>();
            _initialLevel = _upgradable.Level;
        }

        private void Start()
        {
            _currencyAdder = GameStorage.Instance.GetComponent<CurrencyAdder>();
        }

        private void OnMouseDown()
        {
            TrashCanClicked?.Invoke();
        }
    }
}