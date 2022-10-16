using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Enums;

namespace Service
{
    [RequireComponent(typeof(AudioSource))]
    public class MusicManager : MonoBehaviour
    {
        public static MusicManager Instanse { get; private set; }

        [SerializeField] private TypedMusic[] _music;

        private AudioSource _audioSource;

        private Dictionary<Music, AudioClip[]> _musicDictionary = new Dictionary<Music, AudioClip[]>();
        private Music _currentMusic;
        private int _currentIndex = 0;
        private Coroutine _playMusicCoroutine;

        public void Play(Music music)
        {
            _currentMusic = music;
            Stop();
            //_currentIndex = Random.Range(0, _musics[music].Length);
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

            DontDestroyOnLoad(gameObject);

            _audioSource = GetComponent<AudioSource>();

            foreach (var music in _music)
                _musicDictionary.Add(music.Type, music.Clips);

            Play(Music.Game);
        }

        private void OnEnable()
        {
            _playMusicCoroutine = StartCoroutine(CheckMusicEnd());
        }

        private IEnumerator CheckMusicEnd()
        {
            yield return new WaitForSeconds(2f);
            if (!_audioSource.isPlaying)
                Finish();
            else
                _playMusicCoroutine = StartCoroutine(CheckMusicEnd());
        }

        private void Finish()
        {
            if (++_currentIndex >= _musicDictionary[_currentMusic].Length)
                _currentIndex = 0;
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