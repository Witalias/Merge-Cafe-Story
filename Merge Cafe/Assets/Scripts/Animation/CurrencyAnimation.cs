using UnityEngine;
using System.Collections;
using Enums;

namespace AnimationEngine
{
    [RequireComponent(typeof(Animation))]
    public class CurrencyAnimation : MonoBehaviour
    {
        private const string _moveAnimation = "Move";
        private const string _disappearAnimation = "Disappear";
        private const string _idleAnimation = "Idle";
        private const string _appearAnimation = "Appear";

        [SerializeField] private Transform _icon;
        [SerializeField] private Tags _targetTag;
        [SerializeField] private float _minWaitingTime = 1f;
        [SerializeField] private float _maxWaitingTime = 2f;

        [Space]

        [SerializeField] private bool _enableMoveAnimation = true;
        [SerializeField] private bool _enableIdleAnimation = true;
        [SerializeField] private bool _enableAppearAnimation = true;
        [SerializeField] private bool _enableDisappearAnimation = true;

        private Animation _animation;
        private Transform _target;

        private bool _isMoving = false;

        private void Awake()
        {
            _animation = GetComponent<Animation>();
        }

        private void Start()
        {
            _target = GameObject.FindGameObjectWithTag(_targetTag.ToString()).transform;
            Appear();
            StartCoroutine(StartMove());
        }

        private void Update()
        {
            if (_isMoving)
                transform.position = Vector2.Lerp(transform.position, _target.position, 4f * _animation[_moveAnimation].length * Time.deltaTime);

            if (Vector2.Distance(transform.position, _target.position) <= 0.1f)
                Disappear();
        }

        private IEnumerator StartMove()
        {
            yield return new WaitForSeconds(Random.Range(_minWaitingTime, _maxWaitingTime));
            _icon.localScale = new Vector3(1, 1, 1);

            if (_enableMoveAnimation)
                _animation.Play(_moveAnimation);

            _isMoving = true;
        }

        private void Disappear()
        {
            if (_enableDisappearAnimation)
                _animation.Play(_disappearAnimation);
            else
                Destroy(gameObject);
        }

        private void Idle()
        {
            if (_enableIdleAnimation)
            {
                _animation[_idleAnimation].speed = Random.Range(0.7f, 1.3f);
                _animation.Play(_idleAnimation);
            }
        }

        private void Appear()
        {
            if (_enableAppearAnimation)
            {
                _animation[_appearAnimation].speed = Random.Range(0.7f, 1.3f);
                _animation.Play(_appearAnimation);
            }
            else
                Idle();
        }
    }
}
