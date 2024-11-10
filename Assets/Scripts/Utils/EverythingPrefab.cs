using System;
using UnityEngine;

namespace Utils
{
    public class EverythingPrefab: MonoBehaviour
    {
        public static EverythingPrefab Instance { get; private set; }

        private void Start()
        {
            DontDestroyOnLoad(this);
            Instance = this;

        }
    }
}