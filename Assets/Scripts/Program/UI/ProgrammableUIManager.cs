using System;
using System.Linq;
using Controller;
using Level;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using TMP_Text = TMPro.TMP_Text;

namespace Program.UI
{
    public class ProgrammableUIManager: MonoBehaviour
    {
        [SerializeField]
        private AudioClip terminalButtonClickedSound;

        [Space(20)]
        [SerializeField]
        [FormerlySerializedAs("outerTerminalObject")]
        private GameObject terminalObject;
        [SerializeField]
        [FormerlySerializedAs("terminalButtonContainerObject")] 
        private GameObject terminalButtonContainer;

        [Space(20)] 
        [SerializeField]
        private Sprite selectedSprite;
        [SerializeField]
        private Sprite unselectedSprite;

        [Space(20)]
        [Header("Object Description")]
        [SerializeField]
        private Transform objectDescriptionHeader;
        [SerializeField]
        private Transform objectDescriptionBody;

        [SerializeField]
        private Sprite emitterIcon;
        [SerializeField]
        private Sprite executorIcon;

        [Header("Action Description")]
        [SerializeField]
        private Transform actionDescription;

        [Space(20)]
        [Header("Prefabs")]
        [SerializeField]
        private GameObject buttonPrefab;

        [SerializeField]
        private GameObject enumSelectionPrefab;
        [SerializeField]
        private GameObject floatSelectionPrefab;
        
        public static ProgrammableUIManager Instance { get; private set; }

        private bool _isEditing;
        private IActionContainer _currentlyEditing;
        private ActionData _selectedAction;
        
        private void Awake()
        {
            terminalObject.SetActive(false);
            
            Instance = this;
        }

        public void OpenFor(IActionContainer obj)
        {
            _currentlyEditing = obj;
            _isEditing = true;
            terminalObject.SetActive(true);
            _selectedAction = obj.SelectedAction;
            BuildInitialTerminalMenu();

            CameraController.Instance.VisualEditing.Editing = true;
        }

        public void Close()
        {
            if (!_isEditing)
                return;
            _isEditing = false;
            _currentlyEditing.SelectedAction = _selectedAction;
            _currentlyEditing.Begin(_selectedAction);
            terminalObject.SetActive(false);
            _currentlyEditing = null;

            CameraController.Instance.VisualEditing.Editing = false;
        }

        private void Update()
        {
            if (!_isEditing)
                return;

            if (!Input.GetKeyDown(KeyCode.Escape)) return;
            
            Close();
        }

        private void BuildInitialTerminalMenu()
        {
            var containerTransform = terminalButtonContainer.transform;
            while (containerTransform.childCount > 0)
            {
                DestroyImmediate(containerTransform.GetChild(0).gameObject);
            }

            var editObj = _currentlyEditing as MonoBehaviour;

            var idx = 0;
            var blacklist = editObj.TryGetComponent<BlacklistActions>(out var bl)
                ? bl.BlacklistedActions
                : new int[] { };

            foreach (var action in _currentlyEditing.SupportedActions)
            {
                if (blacklist.Contains(idx))
                {
                    idx++;
                    // spawning empty object
                    Instantiate(new GameObject(), containerTransform);
                    continue;
                }

                // Handling button creation
                var newButton = Instantiate(buttonPrefab, containerTransform);
                var buttonTransform = newButton.transform;
                
                // First child is the selection indicator
                var selectionIndicator = buttonTransform.GetChild(0).GetComponent<Image>();
                selectionIndicator.sprite = _selectedAction.ActionIndex == idx ? selectedSprite : unselectedSprite;

                // Second child is the text component
                var text = buttonTransform.GetChild(1).GetComponent<TMP_Text>();
                text.text = action.ActionName;
                
                var buttonCallback = buttonTransform.GetComponent<TerminalCallbackButton>();
                var idxCl = idx;
                buttonCallback.ClickHandler = () =>
                {
                    if (idxCl == _selectedAction.ActionIndex)
                        return;
                    // Handling selection change
                    var previousBtn = containerTransform.GetChild(_selectedAction.ActionIndex);
                    if (previousBtn.childCount != 0)
                    {
                        var btn = previousBtn.GetChild(0).GetComponent<Image>();
                        btn.sprite = unselectedSprite;
                    }

                    selectionIndicator.sprite = selectedSprite;
                    
                    _selectedAction.ActionIndex = idxCl;

                    BuildSelectedActionSubmenu(action);
                    
                    SfxManager.Instance.Play(terminalButtonClickedSound, 0.3f);
                };
                
                idx++;
            }
            
            var rect = containerTransform.parent.GetComponentInParent<ScrollRect>();
            rect.verticalNormalizedPosition = 0f;

            objectDescriptionHeader.GetChild(0).GetComponent<TMP_Text>().text = _currentlyEditing.Name;
            objectDescriptionHeader.GetChild(1).GetComponent<Image>().sprite =
                _currentlyEditing.Type == ProgrammableType.Emitter ? emitterIcon : executorIcon;
            objectDescriptionHeader.GetChild(2).GetComponent<TMP_Text>().text =
                _currentlyEditing.Type == ProgrammableType.Emitter ? "Эмиттер" : "Исполнитель";
            
            objectDescriptionBody.GetComponentInChildren<TMP_Text>().text = _currentlyEditing.Description;
            
            BuildSelectedActionSubmenu(_currentlyEditing.SupportedActions[_currentlyEditing.SelectedAction.ActionIndex]);

            return;

            void BuildSelectedActionSubmenu(ActionInfo action)
            {
                // Handling action description change
                    
                // First child is the name
                actionDescription.GetChild(0).GetComponent<TMP_Text>().text = action.ActionName;

                // Second child is the description
                actionDescription.GetChild(1).GetComponent<TMP_Text>().text = action.ActionDescription;
                    
                // Third child is the empty parameter selection
                var paramSelect = actionDescription.GetChild(2);
                while(paramSelect.childCount > 0)
                    DestroyImmediate(paramSelect.GetChild(0).gameObject);
                    
                switch (action.ValueType)
                {
                    case ActionValueType.Enum:
                        var enumValues = action.EnumType!.GetEnumValues().Cast<object>().AsEnumerable().ToList();
                        var enumSelection = Instantiate(enumSelectionPrefab, paramSelect);
                        var enumText = enumSelection.transform.GetChild(0).GetComponent<TMP_Text>();
                        enumText.text = action.ParameterName!;
                        var dropdown = enumSelection.transform.GetChild(1).GetComponent<TMP_Dropdown>();
                        Debug.Log(enumValues[0]);
                        dropdown.options = enumValues
                            .Select(each => Enum.GetName(action.EnumType!, each))
                            .Select(each => new TMP_Dropdown.OptionData
                            {
                                text = each
                            })
                            .ToList();
                        dropdown.value = _currentlyEditing.SelectedAction.StoredValue == null ? 0 : (int) _currentlyEditing.SelectedAction.StoredValue;
                        _selectedAction.StoredValue = Enum.ToObject(action.EnumType, dropdown.value);
                        dropdown.onValueChanged.AddListener(val =>
                        {
                            Debug.Log(Enum.ToObject(action.EnumType, val));
                            _selectedAction.StoredValue = Enum.ToObject(action.EnumType, val);
                        });
                        break;
                    case ActionValueType.Float:
                        var floatSelection = Instantiate(floatSelectionPrefab, paramSelect).transform;
                        floatSelection.GetChild(0).GetComponent<TMP_Text>().text = action.ParameterName;
                        var slider = floatSelection.transform.GetChild(2).GetComponent<Slider>();
                        var max = floatSelection.GetChild(3).GetComponent<TMP_Text>();
                        max.text = action.MaxFloatValue.ToString();
                        slider.maxValue = action.MaxFloatValue;
                        slider.value = _currentlyEditing.SelectedAction.StoredValue == null ? 0f : (float) _currentlyEditing.SelectedAction.StoredValue;
                        _selectedAction.StoredValue = slider.value;
                        floatSelection.GetChild(1).GetComponent<TMP_Text>().text = Mathf.RoundToInt(slider.value).ToString();
                        slider.onValueChanged.AddListener(val =>
                        {
                            _selectedAction.StoredValue = val;
                            floatSelection.GetChild(1).GetComponent<TMP_Text>().text =
                                Mathf.RoundToInt(val).ToString();
                        });
                        break;
                    case ActionValueType.Unit:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}