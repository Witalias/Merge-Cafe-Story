using Enums;
using Service;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Buttons
{
    public class PlaySoundOnClickButton : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private Sound _sound;

        private void Awake()
        {
            _button.onClick.AddListener(PlaySound);
        }

        private void PlaySound()
        {
            SoundManager.Instanse.Play(_sound, null);
        }
    }
}