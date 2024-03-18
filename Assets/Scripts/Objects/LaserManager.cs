using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Objects
{
    public delegate void MirrorChanged();
    
    [RequireComponent(typeof(AudioSource))]
    public class LaserManager: MonoBehaviour
    {
        public static event MirrorChanged OnMirrorChanged;
        
        [SerializeField]
        private AudioClip laserLoop;
        
        private AudioSource _as;
        private int _activeLasers;
        
        public static LaserManager Instance { get; private set; }
        
        public void Awake()
        {
            _as = GetComponent<AudioSource>();
            _as.volume = 0.05f;
            _as.loop = true;
            _as.playOnAwake = false;
            _as.clip = laserLoop;
            _as.Stop();

            Instance = this;
        }

        public void ActivateLaser()
        {
            if (_activeLasers == 0)
                _as.Play();
            _activeLasers++;
        }

        public void DeactivateLaser()
        {
            _activeLasers--;
            if(_activeLasers <= 0)
                _as.Stop();
        }

        public void Reload()
        {
            // clearing active lasers
            _activeLasers = FindObjectsOfType<LaserEmitter>().Length;
            if(_activeLasers == 0)
                _as.Stop();
        }
    }
}