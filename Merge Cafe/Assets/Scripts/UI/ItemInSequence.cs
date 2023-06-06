using UnityEngine;
using UnityEngine.UI;
using Gameplay.Field;
using Service;
using Enums;
using System.Collections;
using Gameplay.Tutorial;
using System;

namespace UI
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Image))]
    public class ItemInSequence : MonoBehaviour
    {
        private const string _attentionAnimatorTrigger = "Attention";
        private const string _pulsateAnimatorBool = "Pulsate";

        [SerializeField] private Image _reward;
        [SerializeField] private GameObject _cross;
        [SerializeField] private GameObject _ballsParticlePrefab;

        private Animator _animator;
        private Image _image;
        private GameStorage _storage;

        private ItemStorage _rewardStorage;
        private ItemStorage _item;

        public bool ContainsPresent { get; private set; } = false;

        public static event Action PresentGetted;
        public Image GetRewardImage() => _reward;

        public void PlayNewItemParticles() => Instantiate(_ballsParticlePrefab, _reward.transform.position, Quaternion.identity);

        public void PayAttentionAnimation()
        {
            if (_animator != null)
                _animator.SetTrigger(_attentionAnimatorTrigger);
        }

        public void SetPulsate(bool value)
        {
            if (_animator != null)
                _animator.SetBool(_pulsateAnimatorBool, value);
        }

        public void ShowReward(ItemStorage item)
        {
            _storage = GameStorage.Instance;
            _item = item;
            _rewardStorage = _storage.GetRewardForNewItemByLevel(item.Level);
            _reward.gameObject.SetActive(true);
            _reward.sprite = _rewardStorage.Icon;
            ContainsPresent = true;
            if (_item.RewardIsShowing)
                return;
            _item.RewardIsShowing = true;
            PayAttentionAnimation();
            SoundManager.Instanse.Play(Sound.NewItem, null, item.Level - 1);

            // Для синхронизации анимации и звука. На более изящное решение пока нет времени.
            if (item.Level >= 5 && item.Level < 7)
                StartCoroutine(SlowDownAndSpeedUpAnimator(0.5f));
            else if (item.Level >= 7)
                StartCoroutine(SlowDownAndSpeedUpAnimator(1f));

            IEnumerator SlowDownAndSpeedUpAnimator(float delay)
            {
                yield return new WaitForSeconds(0.2f);
                _animator.speed = 0.1f;
                yield return new WaitForSeconds(delay);
                _animator.speed = 1f;
            }
        }

        public void HideReward()
        {
            ContainsPresent = false;
            _reward.gameObject.SetActive(false);
        }

        public void SetSprite(Sprite value) => _image.sprite = value;

        public void SetCrossActive(bool value) => _cross.SetActive(value);

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _image = GetComponent<Image>();
        }

        private void Start()
        {
            _storage = GameStorage.Instance;
        }

        private void OnMouseDown()
        {
            if (!ContainsPresent || (!TutorialSystem.TutorialDone && GetComponent<TutorialTarget>() == null))
                return;

            var randomCell = _storage.GetRandomEmptyCell(true);
            if (randomCell == null)
                return;

            randomCell.CreateItem(_rewardStorage, transform.position);
            _reward.gameObject.SetActive(false);
            ContainsPresent = false;
            _item.RewardIsShowing = false;
            _item.NotNew();

            PresentGetted?.Invoke();
        }
    }
}