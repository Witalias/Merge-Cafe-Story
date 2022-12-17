using Service;
using UnityEngine;

namespace UI.Settings
{
    public class SettingsMenu : MonoBehaviour
    {
        public void SetMusicVolume(float volume)
        {
            MusicManager.Instanse.GetComponent<AudioSource>().volume = volume;
        }
        public void SetSoundVolume(float volume)
        {
            SoundManager.Instanse.GetComponent<AudioSource>().volume = volume;
        }
    }
}