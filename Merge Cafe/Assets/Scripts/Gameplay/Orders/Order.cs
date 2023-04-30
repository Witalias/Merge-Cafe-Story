using UnityEngine;
using UnityEngine.UI;
using Gameplay.Field;
using System.Collections.Generic;
using System.Collections;
using Service;
using Enums;
using TMPro;
using Gameplay.Counters;
using System.Linq;
using Unity.VisualScripting;
using Gameplay.Tutorial;
using System;

namespace Gameplay.Orders
{
    [RequireComponent(typeof(Animator))]
    public class Order : MonoBehaviour, IStorable
    {
        private const string ITEM_TYPE_IN_ORDER_POINT_KEY = "ITEM_TYPE_IN_ORDER_POINT";
        private const string ITEM_LEVEL_IN_ORDER_POINT_KEY = "ITEM_LEVEL_IN_ORDER_POINT";
        private const string EXTRA_REWARD_TYPE_IN_ORDER_KEY = "EXTRA_REWARD_TYPE_IN_ORDER";
        private const string EXTRA_REWARD_LEVEL_IN_ORDER_KEY = "EXTRA_REWARD_LEVEL_IN_ORDER";
        private const string BRILLIANTS_REWARD_IN_ORDER_KEY = "BRILLIANTS_REWARD_IN_ORDER";
        private const string STARS_REWARD_IN_ORDER_KEY = "STARS_REWARD_IN_ORDER";
        private const string _showAnimatorBool = "Show";
        private const string REWARD_COEFFICIENT_KEY = "REWARD_COEFFICIENT";

        [SerializeField] private OrderPoint[] _orderPoints;
        [SerializeField] private Transform _starsSpawnPoint;
        [SerializeField] private Transform _brilliantsSpawnPoint;
        [SerializeField] private GameObject _rewards;
        [SerializeField] private TextMeshProUGUI _starsValueText;
        [SerializeField] private TextMeshProUGUI _brilliantsValueText;
        [SerializeField] private Image _extraRewardImage;
        [SerializeField] private Color _darkenedColor;
        [SerializeField] private float _delayBeforeFinished = 1f;

        private Animator _animator;
        private CurrencyAdder _currencyAdder;

        private readonly ItemStorage[] _orders = new ItemStorage[3];
        private ItemStorage _extraReward;
        private int _id;
        private int _stars;
        private int _brilliants;
        private bool _actived = false;
        private bool started = false;

        private int _reward—oefficient;
        private Color _defaultColor = Color.white;
        private Color _doubledRewardValueColor = Color.green;

        public static event System.Action RewardsAdded;
        public static event System.Action<int> OrderDone;
        public static event System.Action<ItemStorage> NoEmptyCellsAndRewardGetted;

        public ItemStorage[] OrderPoints { get => _orders.Where(point => point != null).ToArray(); }

        public void Save()
        {

            var hierarchyIndex = transform.GetSiblingIndex();
            for (var i = 0; i < _orders.Length; ++i)
            {
                if (_orders[i] == null)
                {
                    PlayerPrefs.DeleteKey(ITEM_TYPE_IN_ORDER_POINT_KEY + i + hierarchyIndex);
                    PlayerPrefs.DeleteKey(ITEM_LEVEL_IN_ORDER_POINT_KEY + i + hierarchyIndex);
                }
                else
                {
                    PlayerPrefs.SetInt(ITEM_TYPE_IN_ORDER_POINT_KEY + i + hierarchyIndex, (int)_orders[i].Type);
                    PlayerPrefs.SetInt(ITEM_LEVEL_IN_ORDER_POINT_KEY + i + hierarchyIndex, _orders[i].Level);
                }
            }
            if (_extraReward == null)
            {
                PlayerPrefs.DeleteKey(EXTRA_REWARD_TYPE_IN_ORDER_KEY + hierarchyIndex);
                PlayerPrefs.DeleteKey(EXTRA_REWARD_LEVEL_IN_ORDER_KEY + hierarchyIndex);
            }
            else
            {
                PlayerPrefs.SetInt(EXTRA_REWARD_TYPE_IN_ORDER_KEY + hierarchyIndex, (int)_extraReward.Type);
                PlayerPrefs.SetInt(EXTRA_REWARD_LEVEL_IN_ORDER_KEY + hierarchyIndex, _extraReward.Level);
            }
            PlayerPrefs.SetInt(BRILLIANTS_REWARD_IN_ORDER_KEY + hierarchyIndex, _brilliants);
            PlayerPrefs.SetInt(STARS_REWARD_IN_ORDER_KEY + hierarchyIndex, _stars);
            PlayerPrefs.SetInt(REWARD_COEFFICIENT_KEY + hierarchyIndex, _reward—oefficient);
        }

        public void Load()
        {
            var hierarchyIndex = transform.GetSiblingIndex();
            var orderPoints = new List<ItemStorage>();
            for (var i = 0; i < _orders.Length; ++i)
            {
                if (PlayerPrefs.HasKey(ITEM_TYPE_IN_ORDER_POINT_KEY + i + hierarchyIndex))
                {
                    var itemType = (ItemType)PlayerPrefs.GetInt(ITEM_TYPE_IN_ORDER_POINT_KEY + i + hierarchyIndex);
                    var level = PlayerPrefs.GetInt(ITEM_LEVEL_IN_ORDER_POINT_KEY + i + hierarchyIndex);
                    orderPoints.Add(GameStorage.Instance.GetItem(itemType, level));
                }
            }
            if (orderPoints.Count == 0 && TutorialSystem.TutorialDone)
            {
                CheckOnEmpty();
                return;
            }
            if (PlayerPrefs.HasKey(EXTRA_REWARD_TYPE_IN_ORDER_KEY + hierarchyIndex))
            {
                var itemType = (ItemType)PlayerPrefs.GetInt(EXTRA_REWARD_TYPE_IN_ORDER_KEY + hierarchyIndex);
                var level = PlayerPrefs.GetInt(EXTRA_REWARD_LEVEL_IN_ORDER_KEY + hierarchyIndex);
                _extraReward = GameStorage.Instance.GetItem(itemType, level);
            }
            _brilliants = PlayerPrefs.GetInt(BRILLIANTS_REWARD_IN_ORDER_KEY + hierarchyIndex);
            _stars = PlayerPrefs.GetInt(STARS_REWARD_IN_ORDER_KEY + hierarchyIndex);
            _reward—oefficient = PlayerPrefs.GetInt(REWARD_COEFFICIENT_KEY + hierarchyIndex, 1);
            Generate(orderPoints.ToArray(), _stars, _brilliants, _reward—oefficient, _extraReward);
        }

        public void SetID(int value) => _id = value;

        public void Generate(ItemStorage[] items, int stars, int brilliants, int reward—oefficient, ItemStorage extraReward)
        {
            _actived = true;

            _stars = stars;
            _brilliants = brilliants;
            _extraReward = extraReward;
            _reward—oefficient = reward—oefficient;
            _starsValueText.text = stars.ToString();
            _brilliantsValueText.text = brilliants.ToString();

            if (extraReward == null)
                _extraRewardImage.gameObject.SetActive(false);
            else
            {
                _extraRewardImage.gameObject.SetActive(true);
                _extraRewardImage.sprite = extraReward.Icon;
            }

            for (var i = 0; i < _orderPoints.Length; ++i)
            {
                if (i < items.Length)
                {
                    if (items[i] == null)
                        break;

                    _orders[i] = items[i];
                    _orderPoints[i].gameObject.SetActive(true);
                    _orderPoints[i].SetItem(items[i]);
                }
                else
                {
                    _orderPoints[i].gameObject.SetActive(false);
                }
            }
            ChangeRewardText();
            Show();
        }

        public void CheckIncomingItem(ItemStorage item, out bool matches)
        {
            matches = false;
            if (IsEmpty())
                return;

            for (var i = 0; i < _orders.Length; ++i)
            {
                if (_orders[i] == null)
                    continue;
                if (_orders[i].EqualTo(item))
                {
                    matches = AcceptIncomingItem(i);
                    break;
                }
            }
            if (IsEmpty())
                GetRewards();
        }

        private void GetRewards()
        {
            _actived = false;
            _currencyAdder.Add(CurrencyType.Star, _stars * _reward—oefficient, _starsSpawnPoint.position);
            _currencyAdder.Add(CurrencyType.Brilliant, _brilliants * _reward—oefficient, _brilliantsSpawnPoint.position);
            GetExtraReward();
            RewardsAdded?.Invoke();
            _rewards.SetActive(false);
            StartCoroutine(Finish(_delayBeforeFinished));
        }

        private bool AcceptIncomingItem(int orderPointIndex)
        {
            SoundManager.Instanse.Play(Sound.Call, null);
            _orderPoints[orderPointIndex].CheckMark.gameObject.SetActive(true);
            _orderPoints[orderPointIndex].CheckMark.Play();
            _orderPoints[orderPointIndex].Icon.color = _darkenedColor;
            _orders[orderPointIndex] = null;
            return true;
        }

        public void ChangeRewardCoefficent(bool isDoublerActive, int ordersToDoubleAmount)
        {
            if (isDoublerActive)
            {
                _reward—oefficient = 2;
            }
            else 
            {
                _reward—oefficient = 1;
            }
            ChangeRewardText();
        }

        private void ChangeRewardText()
        {
  
            _starsValueText.SetText(Convert.ToString(_reward—oefficient * _stars));
            _brilliantsValueText.SetText(Convert.ToString(_reward—oefficient * _brilliants));
            if (_reward—oefficient == 2) // Doubler is active
            {
                _starsValueText.color = _doubledRewardValueColor;
                _brilliantsValueText.color = _doubledRewardValueColor;
            }
            else
            {
                _starsValueText.color = _defaultColor;
                _brilliantsValueText.color = _defaultColor;
            }
        }

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        private void Start()
        {
            _currencyAdder = GameStorage.Instance.GetComponent<CurrencyAdder>();

            if (GameStorage.Instance.LoadData)
                Load();

            started = true;
        }

        private void OnEnable()
        {
            if (!started)
                return;

            if (_actived)
                Show();
            else
                CheckOnEmpty();
        }

        private void Show() => _animator.SetBool(_showAnimatorBool, true);

        private void Hide() => _animator.SetBool(_showAnimatorBool, false);

        private IEnumerator Finish(float delay)
        {
            SoundManager.Instanse.Play(Sound.Cash, null);
            yield return new WaitForSeconds(delay);
            Hide();
            yield return new WaitForSeconds(0.5f);
            foreach (var orderPoint in _orderPoints)
            {
                orderPoint.Icon.color = Color.white;
                orderPoint.CheckMark.gameObject.SetActive(false);
            }
            OrderDone?.Invoke(_id);
            _rewards.SetActive(true);
        }

        private void GetExtraReward()
        {
            if (_extraReward == null)
                return;

            var randomCell = GameStorage.Instance.GetRandomEmptyCell();
            if (randomCell == null)
                NoEmptyCellsAndRewardGetted?.Invoke(_extraReward);
            else
                randomCell.CreateItem(_extraReward, transform.position);
            _extraReward.UnlockFirstly();
        }

        private void CheckOnEmpty()
        {
            if (IsEmpty())
                StartCoroutine(Finish(0f));
        }

        private bool IsEmpty()
        {
            for (var i = 0; i < _orders.Length; ++i)
            {
                if (_orders[i] != null)
                    return false;
            }
            return true;
        }

        [System.Serializable]
        public class ExtraReward
        {
            public ItemType Type;
            public int Level;
            public int MinOrderDifficulty;
        }
    }
}
