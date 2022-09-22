using UnityEngine;
using UnityEngine.UI;
using Gameplay.Field;
using System.Collections.Generic;
using System.Collections;
using System;

namespace Gameplay.Orders
{
    [RequireComponent(typeof(Animator))]
    public class Order : MonoBehaviour
    {
        private const string _showAnimatorBool = "Show";

        [SerializeField] private OrderPoint[] _orderPoints;
        [SerializeField] private Color _darkenedColor;
        [SerializeField] private float _delayBeforeFinished = 1f;

        private Animator _animator;
        private int _id;

        private readonly ItemStats[] _orders = new ItemStats[3];

        public static event Action<int> OrderDone;

        public void SetID(int value) => _id = value;

        public void Generate(ItemStats[] items)
        {
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

        private void Show() => _animator.SetBool(_showAnimatorBool, true);

        private void Hide() => _animator.SetBool(_showAnimatorBool, false);

        private IEnumerator Finish()
        {
            yield return new WaitForSeconds(_delayBeforeFinished);
            Hide();
            yield return new WaitForSeconds(0.5f);
            foreach (var orderPoint in _orderPoints)
            {
                orderPoint.Icon.color = Color.white;
                orderPoint.CheckMark.gameObject.SetActive(false);
            }
            OrderDone?.Invoke(_id);
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
