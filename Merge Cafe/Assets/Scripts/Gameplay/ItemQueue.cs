using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using Gameplay.Field;
using Service;
using Enums;

namespace Gameplay
{
    [RequireComponent(typeof(Animator))]
    public class ItemQueue : MonoBehaviour, IStorable
    {
        private const string ITEM_TYPE_IN_QUEUE_KEY = "ITEM_TYPE_IN_QUEUE";
        private const string ITEM_LEVEL_IN_QUEUE_KEY = "ITEM_LEVEL_IN_QUEUE";
        private const string ITEM_QUEUE_COUNT_KEY = "ITEM_QUEUE_COUNT";
        private const string _twitchAnimatorTrigger = "Twitch";

        [SerializeField] private Image _icon;
        [SerializeField] private GameObject _particles;
        [SerializeField] private AnimationClip _twitchAnimationClip;
        [SerializeField] private float _animationDelay = 5f;

        private Animator _animator;

        private readonly Queue<ItemStorage> _items = new();
        private Coroutine _playAnimationCoroutine;

        public void Save()
        {
            var itemQueue = _items.ToArray();
            for (var i = 0; i < itemQueue.Length; ++i) 
            {
                PlayerPrefs.SetInt(ITEM_TYPE_IN_QUEUE_KEY + i, (int)itemQueue[i].Type);
                PlayerPrefs.SetInt(ITEM_LEVEL_IN_QUEUE_KEY + i, itemQueue[i].Level);
            }
            PlayerPrefs.SetInt(ITEM_QUEUE_COUNT_KEY, itemQueue.Length);
        }

        public void Load()
        {
            var itemQueueCount = PlayerPrefs.GetInt(ITEM_QUEUE_COUNT_KEY, 0);
            for (var i = 0; i < itemQueueCount; ++i)
            {
                var type = (ItemType)PlayerPrefs.GetInt(ITEM_TYPE_IN_QUEUE_KEY + i);
                var level = PlayerPrefs.GetInt(ITEM_LEVEL_IN_QUEUE_KEY + i);
                _items.Enqueue(GameStorage.Instanse.GetItem(type, level));
            }
            if (itemQueueCount > 0)
                Show(_items.Peek().Icon);
        }

        public void Add(ItemStorage item)
        {
            if (_items.Count == 0)
                Show(item.Icon);
            _items.Enqueue(item);
        }

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        private void Start()
        {
            if (GameStorage.Instanse.LoadData)
                Load();
        }

        private void OnMouseDown()
        {
            if (_items.Count == 0)
                return;

            var randomCell = GameStorage.Instanse.GetRandomEmptyCell(true);
            if (randomCell == null)
                return;

            var nextItem = _items.Dequeue();
            randomCell.CreateItem(nextItem, transform.position);

            if (_items.Count == 0)
                Hide();
            else
                SetIcon(_items.Peek().Icon);
        }

        private void Show(Sprite icon)
        {
            _icon.gameObject.SetActive(true);
            SetIcon(icon);
            _particles.SetActive(true);
            _playAnimationCoroutine = StartCoroutine(PlayAnimation());
        }

        private void Hide()
        {
            _icon.gameObject.SetActive(false);
            _particles.SetActive(false);
            StopCoroutine(_playAnimationCoroutine);
        }

        private void SetIcon(Sprite icon) => _icon.sprite = icon;

        private IEnumerator PlayAnimation()
        {
            _animator.SetTrigger(_twitchAnimatorTrigger);
            yield return new WaitForSeconds(_twitchAnimationClip.length + _animationDelay);
            _playAnimationCoroutine = StartCoroutine(PlayAnimation());
        }
    }
}