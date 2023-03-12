using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Enums;

namespace Service
{
    [RequireComponent(typeof(AudioSource))]
    public class MusicManager : MonoBehaviour, IStorable
    {
        public static MusicManager Instanse { get; private set; }

        private const string VOLUME_MUSIC_KEY = "VOLUME_MUSIC";

        [SerializeField] private TypedMusic[] _music;

        private AudioSource _audioSource;
        private readonly Dictionary<Music, AudioClip[]> _musicDictionary = new();
        private Music _currentMusic;
        private int _currentIndex = 0;
        private Coroutine _playMusicCoroutine;

        public void Save()
        {
            PlayerPrefs.SetFloat(VOLUME_MUSIC_KEY, _audioSource.volume);
        }

        public void Load()
        {
            _audioSource.volume = PlayerPrefs.GetFloat(VOLUME_MUSIC_KEY, _audioSource.volume);
        }

        public void Play(Music music)
        {
            _currentMusic = music;
            Stop();
            _currentIndex = 0;
            PlayMusic(_currentIndex);
        }

        public void Stop()
        {
            _audioSource.Stop();
            if (_playMusicCoroutine != null)
                StopCoroutine(_playMusicCoroutine);
        }

        private void Awake()
        {
            if (Instanse == null)
                Instanse = this;
            else
                Destroy(gameObject);

            _audioSource = GetComponent<AudioSource>();

            foreach (var music in _music)
                _musicDictionary.Add(music.Type, music.Clips);

            Play(Music.Game);
        }

        private void Start()
        {
            if (GameStorage.Instanse.LoadData)
                Load();
        }

        private void OnEnable()
        {
            _playMusicCoroutine = StartCoroutine(CheckMusicEnd());
        }

        private IEnumerator CheckMusicEnd()
        {
            yield return new WaitForSeconds(2f);
            if (!_audioSource.isPlaying)
                StartCoroutine(Finish());
            else
                _playMusicCoroutine = StartCoroutine(CheckMusicEnd());
        }

        private IEnumerator Finish()
        {
            if (++_currentIndex >= _musicDictionary[_currentMusic].Length)
                _currentIndex = 0;
            yield return new WaitForSeconds(0f);
            PlayMusic(_currentIndex);
        }

        private void PlayMusic(int index)
        {
            var musicToPlay = _musicDictionary[_currentMusic][index];
            _audioSource.PlayOneShot(musicToPlay);
            _playMusicCoroutine = StartCoroutine(CheckMusicEnd());
        }
    }

    [System.Serializable]
    public class TypedMusic
    {
        public Music Type;
        public AudioClip[] Clips;
    }
}