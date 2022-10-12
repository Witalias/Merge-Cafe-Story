using UnityEngine;
using UnityEngine.UI;
using Gameplay.Field;
using Service;
using Enums;

namespace UI
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Image))]
    public class ItemInSequence : MonoBehaviour
    {
        private const string _attentionAnimatorTrigger = "Attention";

        [SerializeField] private Image _reward;
        [SerializeField] private GameObject _cross;

        private Animator _animator;
        private Image _image;
        private GameStorage _storage;

        private ItemStorage _rewardStorage;

        public bool ContainsPresent { get; private set; } = false;

        public void PayAttentionAnimation()
        {
            _animator.SetTrigger(_attentionAnimatorTrigger);
        }

        public void ShowReward(int itemLevel)
        {
            _storage = GameStorage.Instanse;
            ContainsPresent = true;
            _rewardStorage = _storage.GetRewardForNewItemByLevel(itemLevel);
            _reward.gameObject.SetActive(true);
            _reward.sprite = _rewardStorage.Icon;
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
        }
    }
}