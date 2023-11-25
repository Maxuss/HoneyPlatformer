using System;
using Program.Trigger;
using Unity.VisualScripting;
using UnityEngine;

namespace Program
{
    public class TestProgrammable: Programmable
    {
        private bool _activated;
        
        public override ITrigger[] ApplicableTriggers { get; } =
        {
            new TimeoutTrigger(5f),
            new TimeoutTrigger(1f)
        };
        
        protected override void Activate()
        {
            Debug.Log("TRIGGER EXECUTED");
        }

        protected override void Init()
        {
            SelectedTrigger = ApplicableTriggers[0];
        }
    }
}