using System;
using Dialogue;
using Program.Action;
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

        public override IAction[] ApplicableActions { get; } =
        {
            new TestAction1(),
            new TestAction2()
        };
        
        public DialogueDefinition dialogue;
        
        protected override void Init()
        {
            
        }

        private struct TestAction1: IAction
        {
            public string Name => "Do something";
            public void Execute(Programmable instance)
            {
                instance.StartCoroutine(DialogueManager.Instance.StartDialogue((instance as TestProgrammable)?.dialogue));
            }

            public IAction Copy()
            {
                return new TestAction1();
            }
        }

        private struct TestAction2: IAction
        {
            public string Name => "Do something else";
            public void Execute(Programmable instance)
            {
                Debug.Log("Second action executed");
            }

            public IAction Copy()
            {
                return new TestAction2();
            }
        }
    }
}