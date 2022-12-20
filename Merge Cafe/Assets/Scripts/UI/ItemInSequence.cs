using UnityEngine;
using UnityEngine.UI;
using Gameplay.Field;
using Service;
using Enums;
using System.Collections;

namespace UI
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Image))]
    public class ItemInSequence : MonoBehaviour
    {
        private const string _attentionAnimatorTrigger = "Attention";

        [SerializeField] private Image _reward;
        [SerializeField] private GameObject _cross;
        [SerializeField] private GameObject _ballsParticlePrefab;

        private Animator _animator;
        private Image _image;
        private GameStorage _storage;

        private ItemStorage _rewardStorage;
        private ItemStorage _item;

        public bool ContainsPresent { get; private set; } = false;

        public void PlayNewItemParticles() => Instantiate(_ballsParticlePrefab, _reward.transform.position, Quaternion.identity);

        public void PayAttentionAnimation()
        {
            _animator.SetTrigger(_attentionAnimatorTrigger);
        }

        public void ShowReward(ItemStorage item)
        {
            _storage = GameStorage.Instanse;
            _item = item;
            ContainsPresent = true;
            _rewardStorage = _storage.GetRewardForNewItemByLevel(item.Level);
            _reward.gameObject.SetActive(true);
            _reward.sprite = _rewardStorage.Icon;
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

        public void SetSprite(Sprite value) => _image.sprite = value;

        public void SetCrossActive(bool value) => _cross.SetActive(value);

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _image = GetComponent<Image>();
        }

        private void Start()
        {
            _storage = GameStorage.Instanse;
        }

        private void OnMouseDown()
        {
            if (!ContainsPresent)
                return;

            var randomCell = _storage.GetRandomEmptyCell(true);
            if (randomCell == null)
                return;

            randomCell.CreateItem(_rewardStorage, transform.position);
            _reward.gameObject.SetActive(false);
            ContainsPresent = false;
            _item.NotNew();
        }
    }
}