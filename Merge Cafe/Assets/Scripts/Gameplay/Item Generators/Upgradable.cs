using UnityEngine;
using UnityEngine.UI;
using Gameplay.Field;
using Service;
using Enums;
using System.Collections;
using System;

namespace Gameplay.ItemGenerators
{
    [RequireComponent(typeof(Animator))]
    public class Upgradable : MonoBehaviour
    {
        private const string _burnAnimatorTrigger = "Burn";

        [SerializeField] private Image _image;
        [SerializeField] private GameObject _particles;
        [SerializeField] private ItemType _type;
        [SerializeField] private int _level = 1;

        private Animator _animator;

        public int Level { get => _level; }

        public ItemType Type { get => _type; }

        public static event Action Upgraded;
        public static event Action<ItemType, int> CursorHoveredGenerator;
        public static event Action CursorLeftGenerator;

        public void Activate()
        {
            if (gameObject.activeSelf)
                return;

            gameObject.SetActive(true);
            _animator.SetTrigger(_burnAnimatorTrigger);
        }

        public bool CheckOnUpgrading(ItemStorage item)
        {
            if (_level == GameStorage.Instanse.GetItemMaxLevel(item.Type))
                return false;

            if (item.Type == _type && item.Level == _level)
            {
                Upgrade();
                return true;
            }
            return false;
        }

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _particles.SetActive(false);
        }

        private void Start()
        {
            SetIcon();
        }

        private void OnEnable()
        {
            StartCoroutine(CheckMergingItemOnField());
        }

        private void OnMouseEnter()
        {
            CursorHoveredGenerator?.Invoke(Type, Level);
        }

        private void OnMouseExit()
        {
            CursorLeftGenerator?.Invoke();
        }

        private void Upgrade()
        {
            _animator.SetTrigger(_burnAnimatorTrigger);
            GameStorage.Instanse.RemoveItemsHighlight();
            _particles.SetActive(false);
            ++_level;
            SetIcon();
            SoundManager.Instanse.Play(Sound.UnlockCell, null);
            Upgraded?.Invoke();
        }

        private void SetIcon() => _image.sprite = GameStorage.Instanse.GetItemSprite(_type, _level);

        private IEnumerator CheckMergingItemOnField()
        {
            yield return new WaitForSeconds(2f);
            _particles.SetActive(GameStorage.Instanse.FieldHasItem(GameStorage.Instanse.GetItem(_type, _level)));
            StartCoroutine(CheckMergingItemOnField());
        }
    }
}
