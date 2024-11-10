using System.Collections.Generic;
using Save;
using UnityEngine;

namespace Controller
{
    public class DonController: MonoBehaviour
    {
        private float _lastCall;
        
        public static DonController Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            if (SaveManager.CurrentState.DonUpgrades == null)
                SaveManager.CurrentState.DonUpgrades = new List<DonUpgrade>();
        }
        
    }
}