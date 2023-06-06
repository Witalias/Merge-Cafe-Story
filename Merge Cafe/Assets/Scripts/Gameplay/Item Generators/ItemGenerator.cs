using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UI;
using Service;
using Enums;
using Gameplay.Field;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Gameplay.Tutorial;

namespace Gameplay.ItemGenerators
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Upgradable))]
    public class ItemGenerator : MonoBehaviour, IPointerDownHandler
    {
        private const float _tick = 0.01f;
        private const string _clickAnimatorBool = "Click";

        [SerializeField] private float _boostSpeedMultiplier;
        [SerializeField] private Image _image;
        [SerializeField] private UIBar _bar;
        [SerializeField] private GameObject _energyIcon;
        [SerializeField] private Toggle _toggle;
        [SerializeField] private Transform _itemSpawnPoint;
        [SerializeField] private Color _disabledColor;
        [SerializeField] private Sound _produceSound;
        [SerializeField] private ItemType[] _generatedItems;
        [SerializeField] private GeneratorStats[] _statsOnLevels;

        private Animator _animator;
        private Upgradable _upgradable;
        private GameStorage _storage;
        private Coroutine _addBarValueCoroutine;

        private readonly List<ItemType> _currentGeneratedItems = new();
        private float _currentGenerationTime = 0f;
        private bool _stopped = false;
        private bool _enabled = true;
        private bool _forcedStopped = false;
        private int _remainItemsToSlowingDown = 0;

        public ItemType[] GeneratedItems { get => _generatedItems; }
        public int MaxItemsLevel { get => _statsOnLevels[_upgradable.Level - 1].initItemsLevel; }

        public static event System.Action GeneratorClicked;
        public static event System.Action ItemProduced;
        public static event System.Action Speeded;

        public Toggle GetToggle() => _toggle;

        public void SpeedUp(int onItemCount)
        {
            _remainItemsToSlowingDown += onItemCount;
            _energyIcon.SetActive(true);
            SoundManager.Instanse.Play(Sound.SpeedUp, null);
            Speeded?.Invoke();
        }

        public void SetActiveTimer(bool value)
        {
            if (!gameObject.activeSelf)
                return;
            _forcedStopped = !value;
            if (value == true)
                _addBarValueCoroutine = StartCoroutine(AddBarValue());
            else if (_addBarValueCoroutine != null)
                StopCoroutine(_addBarValueCoroutine);
        }

        public void UpdateProducedItems()
        {
            _currentGeneratedItems.Clear();
            var settings = GameStage.GetSettingsByStage(_storage.GameStage);
            if (settings == null)
                settings = GameStage.GetSettingsByLastStage();
            foreach (var item in settings.Items)
            {
                if (System.Array.Exists(_generatedItems, type => type == item.Type))
                    _currentGeneratedItems.Add(item.Type);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (_stopped || !_enabled || 
                (!TutorialSystem.TutorialDone && GetComponent<TutorialTarget>() == null))
                return;

            GeneratorClicked?.Invoke();

            _animator.SetTrigger(_clickAnimatorBool);
            _currentGenerationTime += _statsOnLevels[_upgradable.Level - 1].clickIncreaseSeconds;
            UpdateBar();
            CheckBarFilled();
        }

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _upgradable = GetComponent<Upgradable>();

            _toggle.onValueChanged.AddListener(OnChangeToggleValue);
        }

        private void Start()
        {
            if (_storage == null)
                _storage = GameStorage.Instance;
            UpdateProducedItems();
        }

        private void OnEnable()
        {
            _storage = GameStorage.Instance;
            if (!_stopped)
                _addBarValueCoroutine = StartCoroutine(AddBarValue());
        }

        private void Update()
        {
            if (_stopped && _enabled && !_forcedStopped && _storage.GetFirstEmptyCell() != null)
            {
                _stopped = false;
                _addBarValueCoroutine = StartCoroutine(AddBarValue());
            }
        }

        private IEnumerator AddBarValue()
        {
            _currentGenerationTime += _tick * (_remainItemsToSlowingDown > 0 ? _boostSpeedMultiplier : 1f);
            UpdateBar();

            yield return new WaitForSeconds(_tick);

            CheckBarFilled();
            if (!_stopped && _enabled)
                _addBarValueCoroutine = StartCoroutine(AddBarValue());
        }

        private void UpdateBar()
        {
            _bar.SetValue(_currentGenerationTime / _statsOnLevels[_upgradable.Level - 1].generationTimeInSec * 100f);
        }

        private void CheckBarFilled()
        {
            var reachedTime = _statsOnLevels[_upgradable.Level - 1].generationTimeInSec;
            if (_currentGenerationTime >= reachedTime)
            {
                CreateItem();
                if (!_stopped && _enabled)
                {
                    _currentGenerationTime -= reachedTime;
                    if (_remainItemsToSlowingDown > 0)
                        --_remainItemsToSlowingDown;
                    CheckBoost();
                }
            }
        }

        private void CreateItem()
        {
            var cell = _storage.GetFirstEmptyCell();
            if (cell == null)
            {
                StopCoroutine(_addBarValueCoroutine);
                _stopped = true;
                return;
            }
            var randomType = _generatedItems[0];
            if (_currentGeneratedItems.Count > 0)
                randomType = _currentGeneratedItems[Random.Range(0, _currentGeneratedItems.Count)];

            var randomLevel = Random.Range(1, _statsOnLevels[_upgradable.Level - 1].initItemsLevel + 1);
            var item = new ItemStorage(_storage.GetItem(randomType, TutorialSystem.TutorialDone ? randomLevel : 1));
            cell.CreateItem(item, _itemSpawnPoint.position);
            _animator.SetTrigger(_clickAnimatorBool);
            SoundManager.Instanse.Play(_produceSound, null);
            ItemProduced?.Invoke();
        }

        private void CheckBoost()
        {
            if (_remainItemsToSlowingDown <= 0)
                _energyIcon.SetActive(false);
        }

        private void OnChangeToggleValue(bool value)
        {
            _enabled = value;
            _image.color = value ? Color.white : _disabledColor;
            SoundManager.Instanse.Play(Sound.Switch, null);
            if (value)
                _addBarValueCoroutine = StartCoroutine(AddBarValue());
        }

        [System.Serializable]
        public class GeneratorStats
        {
            public float generationTimeInSec;
            public float clickIncreaseSeconds;
            public int initItemsLevel;
        }
    }
}