using System;
using Program.Trigger;
using UnityEngine;

namespace Program
{
    public abstract class Programmable: MonoBehaviour
    {
        public abstract ITrigger[] ApplicableTriggers { get; }
        
        public GameObject wiredObject;
        
        protected ITrigger SelectedTrigger;

        private void Start()
        {
            Init();
            SelectedTrigger?.Start();
        }

        protected virtual void Init()
        {
            
        }

        private void Update()
        {
            if (!SelectedTrigger.ShouldTrigger(this)) return;
            
            SelectedTrigger.Activated();
            Activate();
        }

        protected abstract void Activate();
    }
}