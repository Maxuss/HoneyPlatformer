using System;
using DG.Tweening;
using UnityEngine;
using Utils;

namespace DonDomain
{
    public class DonAudioManager: MonoBehaviour
    {
        private AudioSource _unlockAs;

        [SerializeField]
        private AudioClip loadClip;

        [SerializeField]
        private AudioClip doneClip;

        private Tween loadSound;
        
        public static DonAudioManager Instance { get; private set; }

        private AudioSource _musicAs;
        
        private void Start()
        {
            if (TryGetComponent<AudioSource>(out var musicAs))
            {
                _musicAs = musicAs;
                musicAs.DOFade(0.3f * SettingManager.Instance.MusicVolume, 6f).Play();
            }

            _unlockAs = gameObject.AddComponent<AudioSource>();
            _unlockAs.clip = loadClip;

            Instance = this;
        }

        public void FadeOut()
        {
            _musicAs?.DOFade(0f, 0.5f);
        }

        public void PlayDone()
        {
            _unlockAs.Stop();
            _unlockAs.PlayOneShot(doneClip, SettingManager.Instance.SfxVolume);
        }

        public void BeginLoad()
        {
            _unlockAs.volume = 1f * SettingManager.Instance.SfxVolume;
            _unlockAs.Play();
        }

        public void StopLoad()
        {
            if(loadSound != null)
                loadSound.Complete();
            
            loadSound = _unlockAs.DOFade(0f, 0.1f).OnComplete(() =>
            {
                loadSound = null;
                _unlockAs.Stop();
                _unlockAs.time = 0;
            });
        }
    }
}