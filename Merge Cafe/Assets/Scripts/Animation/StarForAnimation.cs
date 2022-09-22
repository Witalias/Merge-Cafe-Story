using UnityEngine;
using System.Collections;
using Enums;

namespace AnimationEngine
{
    [RequireComponent(typeof(Animation))]
    public class StarForAnimation : MonoBehaviour
    {
        private const string _moveAnimation = "Move";
        private const string _disappearAnimation = "Disappear";
        private const string _idleAnimation = "Idle";
        private const string _appearAnimation = "Appear";

        [SerializeField] private Transform _icon;
        [SerializeField] private float _minWaitingTime = 1f;
        [SerializeField] private float _maxWaitingTime = 2f;

        private Animation _animation;
        private Transform _starIcon;

        private bool _isMoving = false;

        private void Awake()
        {
            _animation = GetComponent<Animation>();
        }

        private void Start()
        {
            _starIcon = GameObject.FindGameObjectWithTag(Tags.StarIcon.ToString()).transform;
            Appear();
            StartCoroutine(StartAnimation());
        }

        private void Update()
        {
            if (_isMoving)
                transform.position = Vector2.Lerp(transform.position, _starIcon.position, 4f * _animation[_moveAnimation].length * Time.deltaTime);

            if (Vector2.Distance(transform.position, _starIcon.position) <= 0.1f)
                Disappear();
        }

        private IEnumerator StartAnimation()
        {
            yield return new WaitForSeconds(Random.Range(_minWaitingTime, _maxWaitingTime));
            _icon.localScale = new Vector3(1, 1, 1);
            _animation.Play(_moveAnimation);
            _isMoving = true;
        }

        private void Disappear() => _animation.Play(_disappearAnimation);

        private void Idle() => _animation.Play(_idleAnimation);

        private void Appear() => _animation.Play(_appearAnimation);
    }
}
