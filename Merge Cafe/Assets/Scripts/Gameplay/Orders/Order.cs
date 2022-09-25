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
        [SerializeField] private Transform _brilliantsSpawnLeftTopPoint;
        [SerializeField] private Transform _brilliantsSpawnRightBottomPoint;
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
        private int _brilliants;

        public static event System.Action<int> OrderDone;
        public static event System.Action<int> StarsReceived;
        public static event System.Action<int> BrilliantsReceived;

        public void SetID(int value) => _id = value;

        public void Generate(ItemStats[] items, int stars, int brilliants)
        {
            _stars = stars;
            _brilliants = brilliants;
            _starsValueText.text = stars.ToString();
            _brilliantsValueText.text = brilliants.ToString();

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
            SpawnBrilliants();
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
            BrilliantsReceived?.Invoke(_brilliants);
            OrderDone?.Invoke(_id);
            _rewards.SetActive(true);
        }

        private void SpawnStars() => Spawn(GameStorage.Instanse.StarForAnimation, _stars / 3 + 1, _starsSpawnLeftTopPoint.position, _starsSpawnRightBottomPoint.position);

        private void SpawnBrilliants() => Spawn(GameStorage.Instanse.BrilliantForAnimation, _brilliants / 20 + 1, _brilliantsSpawnLeftTopPoint.position, _brilliantsSpawnRightBottomPoint.position);

        private void Spawn(GameObject obj, int count, Vector2 leftTop, Vector2 rightBottom)
        {
            for (var i = 0; i < count; ++i)
            {
                var randomX = Random.Range(leftTop.x, rightBottom.x);
                var randomY = Random.Range(leftTop.y, rightBottom.y);
                var spawnPoint = new Vector2(randomX, randomY);
                Instantiate(obj, spawnPoint, Quaternion.identity, _mainCanvas);
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
