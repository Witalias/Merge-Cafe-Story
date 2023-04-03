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
    public class Upgradable : MonoBehaviour, IStorable
    {
        private const string UPGRADABLE_GENERATOR_LEVEL_KEY = "UPGRADABLE_GENERATOR_LEVEL_";
        private const string UPGRADABLE_GENERATOR_ACTIVATED_KEY = "UPGRADABLE_GENERATOR_ACTIVATED_";
        private const string _burnAnimatorTrigger = "Burn";

        [SerializeField] private Image _image;
        [SerializeField] private GameObject _particles;
        [SerializeField] private ItemType _type;
        [SerializeField] private int _level = 1;

        private Animator _animator;
        private ItemGenerator _itemGenerator;

        public int Level { get => _level; }

        public ItemType Type { get => _type; }

        public static event Action Upgraded;
        public static event Action<ItemType, int> CursorHoveredGenerator;
        public static event Action CursorLeftGenerator;
        public static event Action<ItemStorage, float> ShowCongratulation;
        public static event Action<ItemType> NewGenerator;

        public void Save()
        {
            PlayerPrefs.SetInt(UPGRADABLE_GENERATOR_LEVEL_KEY + _type, _level);
            PlayerPrefs.SetInt(UPGRADABLE_GENERATOR_ACTIVATED_KEY + _type, gameObject.activeSelf ? 1 : 0);
        }

        public void Load()
        {
            _level = PlayerPrefs.GetInt(UPGRADABLE_GENERATOR_LEVEL_KEY + _type, 1);
            if (PlayerPrefs.GetInt(UPGRADABLE_GENERATOR_ACTIVATED_KEY + _type, 0) == 1)
                Activate();
        }

        public void Activate()
        {
            SetIcon();
            if (gameObject.activeSelf)
                return;

            gameObject.SetActive(true);
            _animator.SetTrigger(_burnAnimatorTrigger);
            NewGenerator?.Invoke(Type);
        }

        public bool CheckIncomingItem(ItemStorage item)
        {
            if (item.Type == ItemType.Energy)
            {
                if (_itemGenerator == null)
                    return false;
                _itemGenerator.SpeedUp(GameStorage.Instance.GetEnergyRewardByItemlevel(item.Level));
                return true;
            }

            if (_level == GameStorage.Instance.GetItemMaxLevel(item.Type))
                return false;

            if (item.Type == _type && item.Level == _level)
            {
                Upgrade();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Warning! Only for tutorial!
        /// </summary>
        public void SetLevel(int level) => _level = level;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _itemGenerator = GetComponent<ItemGenerator>();
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
            GameStorage.Instance.RemoveItemsHighlight();
            _particles.SetActive(false);
            ++_level;
            SoundManager.Instanse.Play(Sound.UnlockCell, null);
            Upgraded?.Invoke();
            SetIcon();
            ShowCongratulation?.Invoke(GameStorage.Instance.GetItem(_type, _level), 1f);
        }

        private void SetIcon() => _image.sprite = GameStorage.Instance.GetItemSprite(_type, _level);

        private IEnumerator CheckMergingItemOnField()
        {
            yield return new WaitForSeconds(2f);
            _particles.SetActive(GameStorage.Instance.FieldHasItem(GameStorage.Instance.GetItem(_type, _level)));
            StartCoroutine(CheckMergingItemOnField());
        }
    }
}
