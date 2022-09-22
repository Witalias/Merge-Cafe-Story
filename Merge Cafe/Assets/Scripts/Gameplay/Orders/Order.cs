using UnityEngine;
using UnityEngine.UI;
using Gameplay.Field;
using System.Collections.Generic;
using System.Collections;
using Service;
using Enums;
using TMPro;

namespace Gameplay.Orders
{
    [RequireComponent(typeof(Animator))]
    public class Order : MonoBehaviour
    {
        private const string _showAnimatorBool = "Show";

        [SerializeField] private OrderPoint[] _orderPoints;
        [SerializeField] private Transform _starsSpawnLeftTopPoint;
        [SerializeField] private Transform _starsSpawnRightBottomPoint;
        [SerializeField] private GameObject _rewards;
        [SerializeField] private TextMeshProUGUI _starsValueText;
        [SerializeField] private TextMeshProUGUI _brilliantsValueText;
        [SerializeField] private Color _darkenedColor;
        [SerializeField] private float _delayBeforeFinished = 1f;

        private Animator _animator;
        private Transform _mainCanvas;

        private readonly ItemStats[] _orders = new ItemStats[3];
        private int _id;
        private int _stars;

        public static event System.Action<int> OrderDone;
        public static event System.Action<int> StarsReceived;

        public void SetID(int value) => _id = value;

        public void Generate(ItemStats[] items, int stars)
        {
            _stars = stars;
            _starsValueText.text = stars.ToString();
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

        public void CheckIncomingItem(ItemStats item, out bool matches)
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
            _mainCanvas = GameObject.FindGameObjectWithTag(Tags.MainCanvas.ToString()).transform;
        }

        private void Show() => _animator.SetBool(_showAnimatorBool, true);

        private void Hide() => _animator.SetBool(_showAnimatorBool, false);

        private IEnumerator Finish()
        {
            SpawnStars();
            _rewards.SetActive(false);
            yield return new WaitForSeconds(_delayBeforeFinished);
            Hide();
            yield return new WaitForSeconds(0.5f);
            foreach (var orderPoint in _orderPoints)
            {
                orderPoint.Icon.color = Color.white;
                orderPoint.CheckMark.gameObject.SetActive(false);
            }
            StarsReceived?.Invoke(_stars);
            OrderDone?.Invoke(_id);
            _rewards.SetActive(true);
        }

        private void SpawnStars()
        {
            var count = _stars / 3 + 1;
            for (var i = 0; i < count; ++i)
            {
                var randomX = Random.Range(_starsSpawnLeftTopPoint.position.x, _starsSpawnRightBottomPoint.position.x);
                var randomY = Random.Range(_starsSpawnLeftTopPoint.position.y, _starsSpawnRightBottomPoint.position.y);
                var spawnPoint = new Vector2(randomX, randomY);
                Instantiate(GameStorage.Instanse.StarForAnimation, spawnPoint, Quaternion.identity, _mainCanvas);
            }
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
    }
}
