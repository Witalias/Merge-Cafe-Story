using UnityEngine;

namespace AnimationEngine
{
    public class RotateAndScale : MonoBehaviour
    {
        [SerializeField] private float _rotationSpeed;
        [SerializeField] private float _scalingSpeed;
        [SerializeField] private float _minScaleValue;
        [SerializeField] private float _maxScaleValue;

        private void Update()
        {
            transform.eulerAngles += _rotationSpeed * Time.deltaTime * transform.forward;

            var scaleValue = Mathf.Lerp(_minScaleValue, _maxScaleValue, Mathf.PingPong(Time.time * _scalingSpeed, 1f));
            transform.localScale = new Vector3(scaleValue, scaleValue, 1f);
        }
    }
}