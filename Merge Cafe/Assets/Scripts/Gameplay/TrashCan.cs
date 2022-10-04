using UnityEngine;
using UnityEngine.UI;
using Gameplay.Counters;
using Gameplay.Field;
using Service;
using Enums;
using System;

namespace Gameplay
{
    public class TrashCan : MonoBehaviour
    {
        [SerializeField] private float[] _brilliantsMultiplierForLevels;
        [SerializeField] private int _level = 2;

        private CurrencyAdder _currencyAdder;
        private Image _image;

        private int _initialLevel;

        public static event Action TrashCanClicked;

        public void Throw(int itemLevel)
        {
            if (_level <= _initialLevel)
            {
                
            }
            else
            {
                var brilliantsCount = (int)(itemLevel * _brilliantsMultiplierForLevels[_level - 3]);
                _currencyAdder.Add(CurrencyType.Brilliant, brilliantsCount, transform.position);
            }
        }

        public bool CheckOnUpgrading(ItemStorage item)
        {
            if (_level == GameStorage.Instanse.GetItemMaxLevel(item.Type))
                return false;

            if (item.Type == ItemType.TrashCan && item.Level == _level)
            {
                Upgrade(item.Icon);
                return true;
            }
            return false;
        }

        private void Awake()
        {
            _image = GetComponent<Image>();
            _initialLevel = _level;
        }

        private void Start()
        {
            _currencyAdder = GameStorage.Instanse.GetComponent<CurrencyAdder>();
        }

        private void OnMouseDown()
        {
            TrashCanClicked?.Invoke();
        }

        private void Upgrade(Sprite icon)
        {
            ++_level;
            _image.sprite = icon;
        }
    }
}