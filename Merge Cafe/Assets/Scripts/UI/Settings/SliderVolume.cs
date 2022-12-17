using UnityEngine;
using UnityEngine.UI;

namespace UI.Settings
{
    public class SliderVolume : MonoBehaviour
    {
        [SerializeField] private AudioSource _source;

        private Slider _slider;

        private void Awake()
        {
            _slider = GetComponent<Slider>();
            _slider.onValueChanged.AddListener(ChangeVolume);

            _slider.value = _source.volume;
        }

        private void ChangeVolume(float value) => _source.volume = value;
    }
}