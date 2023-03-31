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
        [SerializeField] private TextMeshProUGUI _mainText;
        [SerializeField] private TextMeshProUGUI _levelText;
        [SerializeField] private Toggle _shareToggle;
        [SerializeField] private Image _rewardIcon;
        [SerializeField] private Button _confirmButton;
        [SerializeField] private ItemTypeLevel[] _rewardsForNewItems;
        [SerializeField] private ItemTypeLevel[] _randomRewardsForNewGlobalLevels;

        private ItemType _rewardItem;
        private int _rewardLevel;

        public static event Action<ItemStorage> AddItemInQueue;

        public void ShowUpgradedGenerator(ItemStorage generator, float delay)
        {
            var text = $"Вы улучшили «{Translation.GetItemDescription(generator.Type, generator.Level).Title}» и опередили ...% игроков! Выполняйте заказы ещё эффективнее!";
            StartCoroutine(Show(generator.Icon, text, ItemType.Energy, (generator.Level - 1) > 5 ? 5 : generator.Level, delay));
        }

        public void Show(ItemType newGenerator, float delay)
        {
            var text = $"Вы получили новое оборудование «{Translation.GetItemDescription(newGenerator, 1).Title}» и опередили ...% игроков! Ваше меню обновлено!";
            StartCoroutine(Show(GameStorage.Instance.GetItemSprite(newGenerator, 1), text, ItemType.Key, 1, delay));
        }

        public void Show(int globalLevel, float delay)
        {
            if (globalLevel % 5 != 0)
                return;
            var text = $"Вы достигли {globalLevel} уровня и опередили ...% игроков!";
            var randomReward = _randomRewardsForNewGlobalLevels[UnityEngine.Random.Range(0, _randomRewardsForNewGlobalLevels.Length)];
            StartCoroutine(Show(GameStorage.Instance.Star, text, randomReward.Type, randomReward.Level, delay, globalLevel.ToString()));
        }

        public void Show(ItemStorage newItem, float delay)
        {
            if (newItem.Level < 5)
                return;
            var text = $"Вы впервые открыли «{Translation.GetItemDescription(newItem.Type, newItem.Level).Title}» {newItem.Level}-го уровня и опередили ...% игроков!";
            var reward = _rewardsForNewItems[newItem.Level - 5];
            StartCoroutine(Show(newItem.Icon, text, reward.Type, reward.Level, delay));
        }

        public IEnumerator Show(Sprite icon, string text, ItemType rewardItem, int rewardLevel, float delay, string level = "")
        {
            yield return new WaitForSeconds(delay);
            _rewardItem = rewardItem;
            _rewardLevel = rewardLevel;
            _icon.sprite = icon;
            _mainText.text = text;
            _levelText.text = level;
            _rewardIcon.sprite = GameStorage.Instance.GetItemSprite(rewardItem, rewardLevel);
            _shareToggle.isOn = true;
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
            if (_shareToggle.isOn)
                GiveReward();
            _content.SetActive(false);
            Time.timeScale = 1f;
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
        }
    }
}