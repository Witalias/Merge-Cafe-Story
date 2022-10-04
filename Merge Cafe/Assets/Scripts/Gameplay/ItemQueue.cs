using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using Gameplay.Field;
using Service;

namespace Gameplay
{
    [RequireComponent(typeof(Animator))]
    public class ItemQueue : MonoBehaviour
    {
        private const string _twitchAnimatorTrigger = "Twitch";

        [SerializeField] private Image _icon;
        [SerializeField] private GameObject _particles;
        [SerializeField] private AnimationClip _twitchAnimationClip;
        [SerializeField] private float _animationDelay = 5f;

        private Animator _animator;

        private readonly Queue<ItemStorage> _items = new Queue<ItemStorage>();
        private Coroutine _playAnimationCoroutine;

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

        private void OnMouseDown()
        {
            if (_items.Count == 0)
                return;

            var randomCell = GameStorage.Instanse.GetRandomEmptyCell(true);
            if (randomCell == null)
                return;

            randomCell.CreateItem(_items.Dequeue(), transform.position);

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