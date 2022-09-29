using UnityEngine;
using UnityEngine.UI;
using UI;
using TMPro;
using Service;
using Enums;
using System.Collections.Generic;
using Gameplay.Field;

namespace Gameplay.Counters
{
    public class StarCounter : MonoBehaviour
    {
        [SerializeField] private UIBar _bar;
        [SerializeField] private TextMeshProUGUI _value;
        [SerializeField] private Image _nextPresent;
        [SerializeField] private float barSpeed = 1f;

        private GameStorage _storage;

        private List<(ItemType Type, int Level, int RequiredStars)> _targets;
        private (ItemType Type, int Level) _currentTarget;
        private float _currentBarValue = 0f;
        private int _needStarsToNextPresent = 10;

        public static event System.Action<ItemStorage> NoEmptyCellsAndRewardGetted;

        public void AddStars(int value)
        {
            _storage.StatsCount += value;
            UpdateValueText();
        }

        private void Start()
        {
            _storage = GameStorage.Instanse;
            SetTargetsByGameStage();
            UpdateValueText();
        }

        private void Update()
        {
            _currentBarValue = Mathf.Lerp(_currentBarValue, _storage.StatsCount, barSpeed * Time.deltaTime);
            _bar.SetValue(_currentBarValue / _needStarsToNextPresent * 100f);

            if (_bar.Filled)
                FinishTarget();
        }

        private void SetTargetsByGameStage()
        {
            var settings = GameStage.GetSettingsByStage(GameStorage.Instanse.GameStage);
            if (settings == null)
            {
                _nextPresent.gameObject.SetActive(false);
                _needStarsToNextPresent = 999999;
                return;
            }
            _targets = new List<(ItemType Type, int Level, int RequiredStars)>(settings.Targets);
            NextTarget();
        }

        private void UpdateValueText()
        {
            _value.text = $"{_storage.StatsCount} / {_needStarsToNextPresent}";
        }

        private void FinishTarget()
        {
            _currentBarValue = 0f;
            _storage.StatsCount -= _needStarsToNextPresent;

            var itemStorage = _storage.GetItem(_currentTarget.Type, _currentTarget.Level);
            itemStorage.Unlock();

            var randomCell = _storage.GetRandomEmptyCell();
            if (randomCell == null)
                NoEmptyCellsAndRewardGetted?.Invoke(itemStorage);
            else
                randomCell.CreateItem(itemStorage, transform.position);

            if (_targets.Count == 0)
            {
                _storage.GameStage += 1;
                SetTargetsByGameStage();
            }
            else
                NextTarget();

            UpdateValueText();
        }

        private void NextTarget()
        {
            var randomTarget = _targets[Random.Range(0, _targets.Count)];
            _targets.Remove(randomTarget);
            _needStarsToNextPresent = randomTarget.RequiredStars;
            _currentTarget = (randomTarget.Type, randomTarget.Level);
            _nextPresent.sprite = _storage.GetItemSprite(randomTarget.Type, randomTarget.Level);
        }
    }

}