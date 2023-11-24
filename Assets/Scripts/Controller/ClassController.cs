using System;
using UnityEngine;

namespace Controller
{
    public delegate void ClassSwapHandler(ClassController.PlayerClass newClass);
    
    public class ClassController: MonoBehaviour
    {
        public PlayerClass activeClass;

        public static ClassSwapHandler OnClassChange;

        public static ClassController Instance { get; private set; }

        private float _lastChanged;

        public void Awake()
        {
            Instance = this;
        }

        public void Update()
        {
            if (!Input.GetKeyDown(KeyCode.Q) || Time.time - _lastChanged < 1) return;

            _lastChanged = Time.time;
            
            activeClass += 1;
            activeClass = (PlayerClass) ((int) activeClass % 3);
            OnClassChange?.Invoke(activeClass);
        }

        public enum PlayerClass: int
        {
            Engineer,
            Programmer,
            Beekeeper,
        }
    }
}