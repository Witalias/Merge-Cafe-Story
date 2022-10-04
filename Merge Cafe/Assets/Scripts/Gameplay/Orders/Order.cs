using UnityEngine;
using UnityEngine.UI;
using Gameplay.Field;
using System.Collections.Generic;
using System.Collections;
using Service;
using Enums;
using TMPro;
using Gameplay.Counters;

namespace Gameplay.Orders
{
    [RequireComponent(typeof(Animator))]
    public class Order : MonoBehaviour
    {
        private const string _showAnimatorBool = "Show";

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

        public static event System.Action<int> OrderDone;
        public static event System.Action<ItemStorage> NoEmptyCellsAndRewardGetted;

        public void SetID(int value) => _id = value;

        public void Generate(ItemStorage[] items, int stars, int brilliants, ItemStorage extraReward)
        {
            _stars = stars;
            _brilliants = brilliants;
            _extraReward = extraReward;

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
                    _orderPoints[i].CheckMark.gameObject.SetActive(true);
                    _orderPoints[i].CheckMark.Play();
                    _orderPoints[i].Icon.color = _darkenedColor;
                    _orders[i] = null;
                    matches = true;
                    break;
                }
            }
            if (IsEmpty())
                StartCoroutine(Finish());
        }

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        private void Start()
        {
            _currencyAdder = GameStorage.Instanse.GetComponent<CurrencyAdder>();
        }

        private void Show() => _animator.SetBool(_showAnimatorBool, true);

        private void Hide() => _animator.SetBool(_showAnimatorBool, false);

        private IEnumerator Finish()
        {
            _currencyAdder.Add(CurrencyType.Star, _stars, _starsSpawnPoint.position);
            _currencyAdder.Add(CurrencyType.Brilliant, _brilliants, _brilliantsSpawnPoint.position);
            GetExtraReward();
            _rewards.SetActive(false);
            yield return new WaitForSeconds(_delayBeforeFinished);
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

            var randomCell = GameStorage.Instanse.GetRandomEmptyCell();
            if (randomCell == null)
                NoEmptyCellsAndRewardGetted?.Invoke(_extraReward);
            else
                randomCell.CreateItem(_extraReward, transform.position);
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
