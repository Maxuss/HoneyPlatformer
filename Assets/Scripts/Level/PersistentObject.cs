using System;
using UnityEngine;

namespace Level
{
    public class PersistentObject: MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(this);
        }
    }
}