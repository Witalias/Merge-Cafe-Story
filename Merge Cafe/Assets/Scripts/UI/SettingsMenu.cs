using System.Collections;
using System.Collections.Generic;
using Service;
using UnityEngine;

public class SettingsMenu : MonoBehaviour
{
    public void SetMusicVolume(float volume)
    {
        Service.MusicManager.Instanse.GetComponent<AudioSource>().volume=volume;
    }
    public void SetSoundVolume(float volume)
    {
        Service.SoundManager.Instanse.GetComponent<AudioSource>().volume=volume;
    }
}