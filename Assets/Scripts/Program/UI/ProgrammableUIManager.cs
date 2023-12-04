using System;
using Controller;
using Level;
using Program.Action;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Program.UI
{
    public class ProgrammableUIManager: MonoBehaviour
    {
        [SerializeField]
        private AudioClip buttonClickedSound;
        [SerializeField]
        private AudioClip terminalButtonClickedSound;

        [Space(20)]
        [SerializeField]
        private GameObject outerTerminalObject;
        [SerializeField]
        private GameObject terminalButtonContainerObject;

        [Space(20)]
        [SerializeField]
        private GameObject buttonPrefab;

        [SerializeField]
        private Sprite selectedSprite;
        [SerializeField]
        private Sprite unselectedSprite;
        
        public static ProgrammableUIManager Instance { get; private set; }
        public CurrentMenu Menu { get; private set; } = CurrentMenu.Trigger;

        private bool _isEditing = false;
        private Programmable _currentlyEdited;
        private TriggerContainer _selectedCondition;
        private TriggerContainer _selectedAction;
        
        private void Awake()
        {
            outerTerminalObject.SetActive(false);
            
            Instance = this;
        }

        public void OpenFor(Programmable obj)
        {
            PlayerController.Instance.IsDisabled = true;
            
            _currentlyEdited = obj;
            _isEditing = true;
            Menu = CurrentMenu.Trigger;
            // TODO: some open animation?
            outerTerminalObject.SetActive(true);
            _selectedCondition = new TriggerContainer
            {
                Index = obj.SelectedTriggerIndex
            };
            _selectedAction = new TriggerContainer
            {
                Index = obj.SelectedActionIndex,
                FloatData = obj.SelectedAction is IFloatAction floatAction ? floatAction.FloatData : 0f
            };
            RebuildTerminalMenu();
            
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            PlayerController.Instance.IsDisabled = true;
        }

        private void Update()
        {
            if (!_isEditing)
                return;

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                // TODO: some exit animation?

                _isEditing = false;
                _currentlyEdited.SelectAction(_selectedAction.Index, _selectedAction.FloatData);
                _currentlyEdited.SelectTrigger(_selectedCondition.Index);
                outerTerminalObject.SetActive(false);
                _currentlyEdited = null;
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                PlayerController.Instance.IsDisabled = false;
            }
        }

        public void HandleTriggerToggled()
        {
            Menu = CurrentMenu.Trigger;
            SfxManager.Instance.Play(buttonClickedSound, 0.2f);
            RebuildTerminalMenu();
        }

        public void HandleExecutionToggled()
        {
            Menu = CurrentMenu.Execution;
            SfxManager.Instance.Play(buttonClickedSound, 0.2f);
            RebuildTerminalMenu();
        }

        private void RebuildTerminalMenu()
        {
            var containerTransform = terminalButtonContainerObject.transform;
            while (containerTransform.childCount > 0)
            {
                DestroyImmediate(containerTransform.GetChild(0).gameObject);
            }

            if (Menu == CurrentMenu.Execution)
            {
                // Rebuilding execution menu
                var idx = 0;
                foreach (var action in _currentlyEdited.ApplicableActions)
                {
                    var newButton = Instantiate(buttonPrefab, containerTransform);
                    var buttonTransform = newButton.transform;
                    
                    // First child is the selection indicator
                    var selectionIndicator = buttonTransform.GetChild(0).GetComponent<Image>();
                    selectionIndicator.sprite = _selectedAction.Index == idx ? selectedSprite : unselectedSprite;
                    
                    // Second child is the text component
                    var text = buttonTransform.GetChild(1).GetComponent<TMP_Text>();
                    text.text = action.Name;

                    if (action is IFloatAction)
                    {
                        // Third child is the float input
                        var slider = buttonTransform.GetChild(2).GetComponent<Slider>();
                        slider.value = _selectedAction.Index == idx ? _selectedAction.FloatData : 0f;
                        
                        var buttonCallback = buttonTransform.GetComponent<TerminalCallbackButton>();
                        var idxCl = idx;
                        buttonCallback.ClickHandler = () =>
                        {
                            var previousBtn = containerTransform.GetChild(_selectedAction.Index);
                            var btn = previousBtn.GetChild(0).GetComponent<Image>();
                            btn.sprite = unselectedSprite;
                            _selectedAction = new TriggerContainer
                            {
                                Index = idxCl,
                                FloatData = slider.value
                            };
                            containerTransform.GetChild(_selectedAction.Index).GetChild(0).GetComponent<Image>().sprite =
                                selectedSprite;
                            SfxManager.Instance.Play(terminalButtonClickedSound, 0.3f);
                        };
                    }
                    else
                    {
                        // Third child is the float input by default, removing it
                        DestroyImmediate(buttonTransform.GetChild(2).gameObject);

                        var buttonCallback = buttonTransform.GetComponent<TerminalCallbackButton>();
                        var idxCl = idx;
                        buttonCallback.ClickHandler = () =>
                        {
                            var previousBtn = containerTransform.GetChild(_selectedAction.Index);
                            var btn = previousBtn.GetChild(0).GetComponent<Image>();
                            btn.sprite = unselectedSprite;
                            _selectedAction = new TriggerContainer
                            {
                                Index = idxCl
                            };
                            containerTransform.GetChild(_selectedAction.Index).GetChild(0).GetComponent<Image>().sprite =
                                selectedSprite;
                            SfxManager.Instance.Play(terminalButtonClickedSound, 0.3f);
                        };
                    }
                    idx++;
                }
            }
            else
            { 
                // Rebuilding trigger menu
                var idx = 0;
                foreach (var trigger in _currentlyEdited.ApplicableTriggers)
                {
                    var newButton = Instantiate(buttonPrefab, containerTransform);
                    var buttonTransform = newButton.transform;
                    
                    // First child is the selection indicator
                    var selectionIndicator = buttonTransform.GetChild(0).GetComponent<Image>();
                    selectionIndicator.sprite = _selectedAction.Index == idx ? selectedSprite : unselectedSprite;
                    
                    // Second child is the text component
                    var text = buttonTransform.GetChild(1).GetComponent<TMP_Text>();
                    text.text = trigger.Name;

                    // Third child is the float input by default, removing it
                    DestroyImmediate(buttonTransform.GetChild(2).gameObject);

                    var buttonCallback = buttonTransform.GetComponent<TerminalCallbackButton>();
                    var idxCl = idx;
                    buttonCallback.ClickHandler = () =>
                    {
                        var previousBtn = containerTransform.GetChild(_selectedCondition.Index);
                        var btn = previousBtn.GetChild(0).GetComponent<Image>();
                        btn.sprite = unselectedSprite;
                        _selectedCondition = new TriggerContainer
                        {
                            Index = idxCl
                        };
                        containerTransform.GetChild(_selectedCondition.Index).GetChild(0).GetComponent<Image>().sprite =
                            selectedSprite;
                        SfxManager.Instance.Play(terminalButtonClickedSound, 0.3f);
                    };
                    idx++;
                }
            }
        }

        public enum CurrentMenu
        {
            Trigger,
            Execution
        }
        
        private struct TriggerContainer
        {
            public int Index;
            public float FloatData;
        }
    }
}