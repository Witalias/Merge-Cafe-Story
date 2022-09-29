using UnityEngine;
using System.Collections.Generic;
using Enums;

namespace Service
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instanse { get; private set; }

        [SerializeField] private TypedAudio[] _clips;

        private AudioSource _audioSource;

        private Dictionary<Sound, AudioClip[]> _sounds = new Dictionary<Sound, AudioClip[]>();
        private readonly Dictionary<Sound, AudioSource> _playingSounds = new Dictionary<Sound, AudioSource>();

        public void Play(Sound sound, AudioSource source, int index)
        {
            if (!_sounds.ContainsKey(sound))
                return;

            if (source == null)
                source = _audioSource;

            if (_sounds[sound].Length == 0)
            {
                Debug.LogWarning("There is no audio clip " + sound.ToString());
                return;
            }
            if (!_sounds.ContainsKey(sound))
                _playingSounds.Add(sound, source);

            source.PlayOneShot(_sounds[sound][index]);
        }

        public void Play(Sound sound, AudioSource source)
        {
            if (!_sounds.ContainsKey(sound))
                return;
            Play(sound, source, Random.Range(0, _sounds[sound].Length));
        }

        public void PlayOneStream(Sound sound, AudioSource source)
        {
            if (source == null)
                return;

            source.Stop();
            source.PlayOneShot(GetRandomClip(sound));
        }

        public void PlayLoop(Sound sound, AudioSource source)
        {
            if (source == null)
                return;

            source.loop = true;
            _playingSounds.Add(sound, source);
            source.PlayOneShot(GetRandomClip(sound));
        }

        public void Stop(Sound sound)
        {
            if (!_playingSounds.ContainsKey(sound))
                return;

            _playingSounds[sound].Stop();
            _playingSounds.Remove(sound);
        }

        private void Awake()
        {
            if (Instanse == null)
                Instanse = this;
            else
                Destroy(gameObject);

            DontDestroyOnLoad(gameObject);

            _audioSource = GetComponent<AudioSource>();

            foreach (var clip in _clips)
                _sounds.Add(clip.Type, clip.Clips);
        }

        private void Update()
        {
            foreach (var element in _playingSounds)
            {
                if (element.Value.isPlaying)
                    continue;

                element.Value.PlayOneShot(GetRandomClip(element.Key));
            }
        }

        private AudioClip GetRandomClip(Sound sound) => _sounds[sound][Random.Range(0, _sounds[sound].Length)];
    }

    [System.Serializable]
    public class TypedAudio
    {
        public Sound Type;
        public AudioClip[] Clips;
    }

}