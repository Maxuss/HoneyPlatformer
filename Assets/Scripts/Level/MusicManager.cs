using System;
using System.Collections;
using UnityEngine;
using Utils;

namespace Level
{
    public class MusicManager: MonoBehaviour
    {
        private AudioSource _ambient1, _ambient2;
        private int _currentAmbientTrack;
        
        [SerializeField]
        private AudioClip[] ambientTracks;
        
        public MusicManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;

            _ambient1 = gameObject.AddComponent<AudioSource>();
            _ambient1.volume = 0f;
            _ambient1.playOnAwake = false;
            _ambient2 = gameObject.AddComponent<AudioSource>();
            _ambient2.volume = 0f;
            _ambient2.playOnAwake = false;

            StartCoroutine(Util.Delay(NextAmbientTrack, 2f));
        }
        
        [ContextMenu("Next Ambient")]
        public void NextAmbientTrack()
        {
            var track = ambientTracks[_currentAmbientTrack];
            StartCoroutine(Crossfade(track, .3f, .05f));
            _currentAmbientTrack++;
            _currentAmbientTrack %= ambientTracks.Length;
            Invoke(nameof(NextAmbientTrack), track.length - 2f);
        }
        
        private IEnumerator Crossfade(AudioClip to, float maxVolume = 1f, float speed = .2f)
        {
            if (_ambient1.volume > 0)
            {
                _ambient2.clip = to;
                _ambient2.volume = 0f;
                _ambient2.Play();
                var deltaVolume = 0f;

                while (deltaVolume < maxVolume)
                {
                    deltaVolume += speed * Time.deltaTime;
                    _ambient1.volume = maxVolume - deltaVolume;
                    _ambient2.volume = deltaVolume;
                    yield return null;
                }

                _ambient1.Stop();

                _ambient2.volume = maxVolume;
                _ambient1.volume = 0f;
            }
            else
            {
                _ambient1.clip = to;
                _ambient1.volume = 0f;
                _ambient1.Play();
                var deltaVolume = 0f;

                while (deltaVolume < maxVolume)
                {
                    deltaVolume += speed * Time.deltaTime;
                    _ambient2.volume = maxVolume - deltaVolume;
                    _ambient1.volume = deltaVolume;
                    yield return null;
                }

                _ambient2.Stop();

                _ambient1.volume = maxVolume;
                _ambient2.volume = 0f;
            }
        }
    }
}