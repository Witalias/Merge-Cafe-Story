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
    public class StarCounter : MonoBehaviour, IStorable
    {
        private const string CURRENT_STAR_REWARD_TYPE_KEY = "CURRENT_STAR_REWARD_TYPE";
        private const string CURRENT_STAR_REWARD_LEVEL_KEY = "CURRENT_STAR_REWARD_LEVEL";
        private const string STAR_REWARD_TYPE_KEY = "STAR_REWARD_TYPE";
        private const string STAR_REWARD_LEVEL_KEY = "STAR_REWARD_LEVEL";
        private const string STAR_REWARD_REQUIRED_STARS_KEY = "STAR_REWARD_REQUIRED_STARS";
        private const string STAR_REWARD_COUNT_KEY = "STAR_REWARD_COUNT";
        private const string NEED_STARS_TO_NEXT_REWARD_KEY = "NEED_STARS_TO_NEXT_REWARD";
        private const int _cheatStarsCount = 20;

        [SerializeField] private UIBar _bar;
        [SerializeField] private TextMeshProUGUI _value;
        [SerializeField] private Image _nextPresent;
        [SerializeField] private float barSpeed = 1f;

        private GameStorage _storage;

        private List<(ItemType Type, int Level, int RequiredStars)> _rewards;
        private (ItemType Type, int Level) _currentReward;
        private float _currentBarValue = 0f;
        private int _needStarsToNextReward = 10;

        public static event System.Action<ItemStorage> NoEmptyCellsAndRewardGetted;
        public static event System.Action UpdateOrderCount;
        public static event System.Action NewStageReached;
        public static event System.Action<ItemType> ActivateGenerator;
        public static event System.Func<ItemType, bool> IsGenerator;
        public static event System.Func<ItemType, bool> GeneratorExistsInGame;

        public void Save()
        {
            for (var i = 0; i < _rewards.Count; ++i)
            {
                PlayerPrefs.SetInt(STAR_REWARD_TYPE_KEY + i.ToString(), (int)_rewards[i].Type);
                PlayerPrefs.SetInt(STAR_REWARD_LEVEL_KEY + i.ToString(), _rewards[i].Level);
                PlayerPrefs.SetInt(STAR_REWARD_REQUIRED_STARS_KEY + i.ToString(), _rewards[i].RequiredStars);
            }
            PlayerPrefs.SetInt(STAR_REWARD_COUNT_KEY, _rewards.Count);
            PlayerPrefs.SetInt(NEED_STARS_TO_NEXT_REWARD_KEY, _needStarsToNextReward);
            PlayerPrefs.SetInt(CURRENT_STAR_REWARD_TYPE_KEY, (int)_currentReward.Type);
            PlayerPrefs.SetInt(CURRENT_STAR_REWARD_LEVEL_KEY, _currentReward.Level);
        }

        public void Load()
        {
            _rewards = new List<(ItemType Type, int Level, int RequiredStars)>();
            var count = PlayerPrefs.GetInt(STAR_REWARD_COUNT_KEY, 0);
            for (var i = 0; i < count; ++i)
            {
                (ItemType, int, int) reward = (
                    (ItemType)PlayerPrefs.GetInt(STAR_REWARD_TYPE_KEY + i.ToString()),
                    PlayerPrefs.GetInt(STAR_REWARD_LEVEL_KEY + i.ToString()),
                    PlayerPrefs.GetInt(STAR_REWARD_REQUIRED_STARS_KEY + i.ToString()));
                _rewards.Add(reward);
            }
            _currentReward = (
                (ItemType)PlayerPrefs.GetInt(CURRENT_STAR_REWARD_TYPE_KEY),
                PlayerPrefs.GetInt(CURRENT_STAR_REWARD_LEVEL_KEY, 1));
            _needStarsToNextReward = PlayerPrefs.GetInt(NEED_STARS_TO_NEXT_REWARD_KEY, 1);
        }

        public void AddStars(int value)
        {
            _storage.StarsCount += value;
            UpdateValueText();
        }

        private void Start()
        {
            _storage = GameStorage.Instanse;

            if (_storage.LoadData)
                Load();

            if (_rewards == null)
                SetRewardsByGameStage();
            else
                SetNextRewardSprite(_currentReward);

            UpdateValueText();
        }

        private void Update()
        {
            _currentBarValue = Mathf.Lerp(_currentBarValue, _storage.StarsCount, barSpeed * Time.deltaTime);
            _bar.SetValue(_currentBarValue / _needStarsToNextReward * 100f);

            if (_bar.Filled)
                FinishTarget();

            if (Input.GetKeyDown(KeyCode.S))
                AddStars(_cheatStarsCount);
        }

        private void SetRewardsByGameStage()
        {
            var settings = GameStage.GetSettingsByStage(GameStorage.Instanse.GameStage);
            if (settings == null)
            {
                _nextPresent.gameObject.SetActive(false);
                _needStarsToNextReward = 999999;
                return;
            }
            if (_storage.OrdersCountMustBeUpdated)
                UpdateOrderCount?.Invoke();
            NewStageReached?.Invoke();
            _rewards = new List<(ItemType Type, int Level, int RequiredStars)>(settings.Targets);
            NextReward();
        }

        private void UpdateValueText()
        {
            _value.text = $"{_storage.StarsCount} / {_needStarsToNextReward}";
        }

        private void FinishTarget()
        {
            _currentBarValue = 0f;
            _storage.StarsCount -= _needStarsToNextReward;

            var itemStorage = _storage.GetItem(_currentReward.Type, _currentReward.Level);
            itemStorage.Unlock();

            CheckOnNewGenerator(itemStorage.Type, out bool isNewGenerator);

            if (!isNewGenerator)
            {
                var randomCell = _storage.GetRandomEmptyCell();
                if (randomCell == null)
                    NoEmptyCellsAndRewardGetted?.Invoke(itemStorage);
                else
                    randomCell.CreateItem(itemStorage, transform.position);
            }

            if (_rewards.Count == 0)
            {
                _storage.GameStage += 1;
                SetRewardsByGameStage();
            }
            else
                NextReward();

            UpdateValueText();
        }

        private void NextReward()
        {
            var randomReward = _rewards[Random.Range(0, _rewards.Count)];
            _rewards.Remove(randomReward);
            SetCurrentReward(randomReward);
        }

        private void SetCurrentReward((ItemType Type, int Level, int RequiredStars) reward)
        {
            _needStarsToNextReward = reward.RequiredStars;
            _currentReward = (reward.Type, reward.Level);
            SetNextRewardSprite(_currentReward);
        }

        private void SetNextRewardSprite((ItemType Type, int Level) currentReward) => 
            _nextPresent.sprite = _storage.GetItemSprite(currentReward.Type, currentReward.Level);

        private void CheckOnNewGenerator(ItemType type, out bool isNew)
        {
            var isGenerator = IsGenerator?.Invoke(type);
            if (isGenerator.GetValueOrDefault())
            {
                var existsInGame = GeneratorExistsInGame?.Invoke(type);
                if (!existsInGame.GetValueOrDefault())
                {
                    isNew = true;
                    ActivateGenerator?.Invoke(type);
                    return;
                }
            }
            isNew = false;
        }
    }
}