using System;
using System.Linq;
using Controller;
using Program.Action;
using Program.Trigger;
using Program.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Program
{
    public abstract class Programmable: MonoBehaviour
    {
        public abstract ITrigger[] ApplicableTriggers { get; }
        public abstract IAction[] ApplicableActions { get; }
        
        private bool _editing;
        
        public GameObject wiredObject;
        
        public ITrigger SelectedTrigger { get; private set; }
        public int SelectedTriggerIndex { get; private set; }
        public IAction SelectedAction { get; private set; }
        public int SelectedActionIndex { get; private set; }

        public void SelectTrigger(int triggerIdx)
        {
            SelectedTrigger = ApplicableTriggers[triggerIdx].Copy();
            SelectedTriggerIndex = triggerIdx;
            
            SelectedTrigger?.Start();
        }

        public void SelectAction(int actionIdx, float extraData = 0f)
        {
            SelectedAction = ApplicableActions[actionIdx].Copy();
            SelectedActionIndex = actionIdx;
            if (SelectedAction is IFloatAction floatAction)
                floatAction.FloatData = extraData;
        }

        private void Update()
        {
            if (SelectedTrigger == null || !SelectedTrigger.ShouldTrigger(this)) return;
            
            SelectedTrigger!.Activated();
            SelectedAction.Execute(this);
        }

        public void OnInteract()
        {
            _editing = true;
            PlayerController.Instance.IsDisabled = true;
            
            ProgrammableUIManager.Instance.OpenFor(this);
        }
    }
}