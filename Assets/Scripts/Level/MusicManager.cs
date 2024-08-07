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
        
        public static MusicManager Instance { get; private set; }

        [SerializeField]
        public bool dontPlayAmbient;

        private void Awake()
        {
            Instance = this;

            _ambient1 = gameObject.AddComponent<AudioSource>();
            _ambient1.volume = 0f;
            _ambient1.loop = true;
            _ambient1.playOnAwake = false;
            _ambient2 = gameObject.AddComponent<AudioSource>();
            _ambient2.volume = 0f;
            _ambient2.loop = true;
            _ambient2.playOnAwake = false;
        }
        
        [ContextMenu("Next Ambient")]
        public void NextAmbientTrack()
        {
            if (dontPlayAmbient)
                return;
            var track = ambientTracks[_currentAmbientTrack];
            Debug.Log($"PLAYING TRACK {track}");
            StartCoroutine(Crossfade(track, .18f, .05f));
            _currentAmbientTrack++;
            _currentAmbientTrack %= ambientTracks.Length;
            Invoke(nameof(NextAmbientTrack), track.length - 2f);
        }

        public void StopAbruptly()
        {
            _ambient1.Stop();
            _ambient2.Stop();
            _ambient1.volume = 0f;
            _ambient2.volume = 0f;
        }

        public IEnumerator Crossfade(AudioClip to, float maxVolume = 1f, float speed = .2f, bool repeat = false)
        {
            maxVolume *= SettingManager.Instance.MusicVolume;
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
                _ambient1.loop = repeat;
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
                _ambient2.loop = repeat;
            }
        }
    }
}