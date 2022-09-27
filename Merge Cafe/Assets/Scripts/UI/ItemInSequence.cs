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

        [SerializeField] private Image _present;

        private Animator _animator;
        private Image _image;
        private GameStorage _storage;

        private (ItemType Type, int Level) _presentStats;

        public bool ContainsPresent { get; private set; } = false;

        public void PayAttentionAnimation()
        {
            _animator.SetTrigger(_attentionAnimatorTrigger);
        }

        public void ShowPresent(int level)
        {
            _storage = GameStorage.Instanse;
            ContainsPresent = true;
            var presentLevel = _storage.GetPresentLevelByItemlevel(level);
            _presentStats = (ItemType.Present, presentLevel);
            _present.gameObject.SetActive(true);
            _present.sprite = _storage.GetItemSprite(_presentStats.Type, _presentStats.Level);
        }

        public void SetSprite(Sprite value) => _image.sprite = value;

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

            var randomCell = _storage.GetRandomEmptyCell();
            randomCell.CreateItem(_storage.GetItem(_presentStats.Type, _presentStats.Level), transform.position);
            _present.gameObject.SetActive(false);
            ContainsPresent = false;
        }
    }
}