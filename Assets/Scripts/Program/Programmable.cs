using System;
using System.Linq;
using Controller;
using Program.Action;
using Program.Trigger;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Program
{
    public abstract class Programmable: MonoBehaviour
    {
        public abstract ITrigger[] ApplicableTriggers { get; }
        public abstract IAction[] ApplicableActions { get; }

        [SerializeField]
        private Canvas uiCanvas;
        [SerializeField]
        private TMP_Dropdown selectTrigger;
        [SerializeField]
        private TMP_Dropdown selectAction;

        private bool _editing;
        
        public GameObject wiredObject;
        
        protected ITrigger SelectedTrigger;
        protected IAction SelectedAction;

        private void Start()
        {
            uiCanvas.enabled = false;
            Init();
        }

        protected virtual void Init()
        {
            
        }

        private void Update()
        {
            if (PlayerController.Instance.IsInUi && _editing)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                    _editing = false;
                    PlayerController.Instance.IsInUi = false;
                    uiCanvas.enabled = false;
                    selectTrigger.enabled = false;
                    selectAction.enabled = false;

                    SelectedAction = ApplicableActions[selectAction.value].Copy();
                    SelectedTrigger = ApplicableTriggers[selectTrigger.value].Copy();
                    
                    SelectedTrigger?.Start();
                }
            }
            
            if (SelectedTrigger == null || !SelectedTrigger.ShouldTrigger(this)) return;
            
            SelectedTrigger!.Activated();
            SelectedAction.Execute(this);
        }

        public void OnInteract()
        {
            uiCanvas.enabled = true;
            selectAction.enabled = true;
            selectTrigger.enabled = true;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            _editing = true;
            PlayerController.Instance.IsInUi = true;

            selectTrigger.options = ApplicableTriggers.Select(each => new TMP_Dropdown.OptionData(each.Name)).ToList();
            selectAction.options = ApplicableActions.Select(each => new TMP_Dropdown.OptionData(each.Name)).ToList();
        }
    }
}