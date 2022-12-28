using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Service;
using System;
using Enums;
using Gameplay.Counters;

namespace Gameplay.DecorationMode
{
    [RequireComponent(typeof(Animator))]
    public class PurchaseButton : MonoBehaviour
    {
        private const string _appearAnimatorTrigger = "Appear";

        [SerializeField] private Image _circle;
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _valueText;
        [SerializeField] private GameObject _particles;

        private GameStorage _storage;
        private Animator _animator;

        private Action _purchaseAction;
        private float _initialOpacity;
        private int _cost;

        public static event Action NotEnoughBrilliants;

        public void SetIcon(Sprite icon) => _icon.sprite = icon;

        public void SetCost(int value)
        {
            _cost = value;
            _valueText.text = value.ToString();
        }

        public int ShowCost()
        {
            return _cost;
        }

        public void SetPurchaseAction(Action value) => _purchaseAction = value;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _initialOpacity = _circle.color.a;
            _animator.SetTrigger(_appearAnimatorTrigger);
        }

        private void Start()
        {
            _storage = GameStorage.Instanse;
        }

        private void Update()
        {
            _particles.SetActive(_storage.BrilliantsCount >= _cost);
        }

        private void OnMouseEnter()
        {
            SetCircleOpacity(1f);
        }

        private void OnMouseExit()
        {
            SetCircleOpacity(_initialOpacity);
        }

        private void OnMouseDown()
        {
            if (_storage.BrilliantsCount < _cost)
            {
                NotEnoughBrilliants?.Invoke();
                return;
            }

            SoundManager.Instanse.Play(Sound.Buy, null);
            SoundManager.Instanse.Play(Sound.Magic, null);
            GameStorage.Instanse.GetComponent<CurrencyAdder>().Add(CurrencyType.Brilliant, -_cost);
            _purchaseAction?.Invoke();
            Destroy(gameObject);
        }

        private void SetCircleOpacity(float value) =>
            _circle.color = new Color(_circle.color.r, _circle.color.g, _circle.color.b, value);
    }
}