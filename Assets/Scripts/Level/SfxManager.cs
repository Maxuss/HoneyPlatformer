using System;
using UnityEngine;

namespace Level
{
    [RequireComponent(typeof(AudioSource))]
    public class SfxManager: MonoBehaviour
    {
        private AudioSource _as;

        public static SfxManager Instance { get; private set; }
        
        public void Awake()
        {
            _as = GetComponent<AudioSource>();

            Instance = this;
        }

        public void Play(AudioClip clip, float volumeScale = 1f) => _as.PlayOneShot(clip, volumeScale);
    }
}