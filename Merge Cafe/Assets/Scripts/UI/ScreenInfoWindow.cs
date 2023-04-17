using Enums;
using Gameplay.Field;
using Service;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ScreenInfoWindow : MonoBehaviour
    {
        [SerializeField] private GameObject _content;
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _title;
        [SerializeField] private TextMeshProUGUI _mainText;
        [SerializeField] private TextMeshProUGUI _levelText;
        [SerializeField] private TextMeshProUGUI _viewAdsText;
        [SerializeField] private TextMeshProUGUI _greatText;
        [SerializeField] private Toggle _shareToggle;
        [SerializeField] private Image _rewardIcon;
        [SerializeField] private Button _confirmButton;
        [SerializeField] private ItemTypeLevel[] _rewardsForNewItems;
        [SerializeField] private ItemTypeLevel[] _randomRewardsForNewGlobalLevels;

        private ItemType _rewardItem;
        private int _rewardLevel;

        public static event Action<ItemStorage> AddItemInQueue;
        public static event Action AchivementRewardGetted;

        public void ShowUpgradedGenerator(ItemStorage generator, float delay)
        {
            var parts = Translation.GetUpgradedGeneratorTextParts(GameStorage.Instance.Language);
            var text = $"{parts[0]} «{Translation.GetItemDescription(GameStorage.Instance.Language, generator.Type, generator.Level).Title}»! {parts[1]}!";
            StartCoroutine(Show(generator.Icon, text, ItemType.Energy, (generator.Level - 1) > 5 ? 5 : generator.Level, delay));
        }

        public void Show(ItemType newGenerator, float delay)
        {
            var parts = Translation.GetNewGeneratorTextParts(GameStorage.Instance.Language);
            var text = $"{parts[0]} «{Translation.GetItemDescription(GameStorage.Instance.Language, newGenerator, 1).Title}»! {parts[1]}!";
            StartCoroutine(Show(GameStorage.Instance.GetItemSprite(newGenerator, 1), text, ItemType.Key, 1, delay));
        }

        public void Show(int globalLevel, float delay)
        {
            if (globalLevel % 5 != 0)
                return;
            var parts = Translation.GetNewLevelTextParts(GameStorage.Instance.Language);
            var text = $"{parts[0]} {globalLevel}{parts[1]}!";
            var randomReward = _randomRewardsForNewGlobalLevels[UnityEngine.Random.Range(0, _randomRewardsForNewGlobalLevels.Length)];
            StartCoroutine(Show(GameStorage.Instance.Star, text, randomReward.Type, randomReward.Level, delay, globalLevel.ToString()));
        }

        public void Show(ItemStorage newItem, float delay)
        {
            if (newItem.Level < 5)
                return;
            var parts = Translation.GetNewItemTextParts(GameStorage.Instance.Language);
            var text = $"{parts[0]}«{Translation.GetItemDescription(GameStorage.Instance.Language, newItem.Type, newItem.Level).Title}»{parts[1]}{newItem.Level}{parts[2]}!";
            var reward = _rewardsForNewItems[newItem.Level - 5];
            StartCoroutine(Show(newItem.Icon, text, reward.Type, reward.Level, delay));
        }

        public IEnumerator Show(Sprite icon, string text, ItemType rewardItem, int rewardLevel, float delay, string level = "")
        {
            yield return new WaitForSeconds(delay);
            var language = GameStorage.Instance.Language;
            _rewardItem = rewardItem;
            _rewardLevel = rewardLevel;
            _icon.sprite = icon;
            _title.text = Translation.GetCongratulationsText(language);
            _mainText.text = text;
            _levelText.text = level;
            _viewAdsText.text = Translation.GetViewAdsText(language);
            _greatText.text = Translation.GetGreatText(language);
            _rewardIcon.sprite = GameStorage.Instance.GetItemSprite(rewardItem, rewardLevel);
            _shareToggle.isOn = true;
            _content.SetActive(true);
            yield return new WaitForEndOfFrame();
            _content.SetActive(false);
            _content.SetActive(true);
            Time.timeScale = 0f;
            SoundManager.Instanse.Play(Sound.Achivement, null);
        }

        private void Awake()
        {
            _confirmButton.onClick.AddListener(Hide);
        }

        private void Hide()
        {
            _content.SetActive(false);
            Time.timeScale = 1f;
            if (_shareToggle.isOn)
                GiveReward();
        }

        private void GiveReward()
        {
            var item = GameStorage.Instance.GetItem(_rewardItem, _rewardLevel);
            var randomCell = GameStorage.Instance.GetRandomEmptyCell();
            if (randomCell == null)
                AddItemInQueue?.Invoke(item);
            else
                randomCell.CreateItem(item, _rewardIcon.transform.position);
            item.UnlockFirstly();
            AchivementRewardGetted?.Invoke();
        }
    }
}