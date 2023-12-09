using System;
using System.Collections;
using UnityEngine;

namespace Level
{
    public class MusicManager: MonoBehaviour
    {
        private AudioSource _active, _crossfadeTo;
        private int _currentAmbientTrack;
        
        [SerializeField]
        private AudioClip[] ambientTracks;
        
        public MusicManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;

            _active = gameObject.AddComponent<AudioSource>();
            _active.volume = 0f;
            _active.playOnAwake = false;
            _crossfadeTo = gameObject.AddComponent<AudioSource>();
            _crossfadeTo.volume = 0f;
            _crossfadeTo.playOnAwake = false;
        }

        [ContextMenu("Next Ambient")]
        public void NextAmbientTrack()
        {
            var track = ambientTracks[_currentAmbientTrack];
            StartCoroutine(_currentAmbientTrack == 0 ? FadeIn(track, .3f) : Crossfade(track, .3f));
            _currentAmbientTrack++;
            _currentAmbientTrack %= ambientTracks.Length;
        }

        public IEnumerator FadeIn(AudioClip to, float maxVolume = 1f, float speed = .2f)
        {
            Debug.Log($"HERE {_active.clip}");
            _active.clip = to;
            Debug.Log($"THERE {_active.clip}");
            _active.Play();
            var deltaVolume = 0f;

            while (deltaVolume < maxVolume)
            {
                deltaVolume += speed * Time.deltaTime;
                _active.volume = deltaVolume;
                yield return null;
            }

            _active.volume = maxVolume;
        }

        public IEnumerator Crossfade(AudioClip to, float maxVolume = 1f, float speed = .2f)
        {
            _crossfadeTo.clip = to;
            var deltaVolume = 0f;

            while (deltaVolume < maxVolume)
            {
                deltaVolume += speed * Time.deltaTime;
                _active.volume = 1 - deltaVolume;
                _crossfadeTo.volume = 1 + deltaVolume;
                yield return null;
            }

            _active.volume = maxVolume;
            _active.Stop();
            _active = _crossfadeTo;
            _active.volume = 1f;
            _active.Play();
            _crossfadeTo.Stop();
        }
    }
}