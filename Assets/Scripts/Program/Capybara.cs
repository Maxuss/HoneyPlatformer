using System;
using Controller;
using Dialogue;
using UnityEngine;
using UnityEngine.UIElements;
using Utils;

namespace Program
{
    public class Capybara: MonoBehaviour
    {
        [SerializeField] private DialogueDefinition dialogue;
        
        private bool _shown;

        private void OnMouseOver()
        {
            if (_shown)
                return;
            if (Input.GetMouseButton((int)MouseButton.LeftMouse) || Input.GetMouseButton((int) MouseButton.RightMouse))
            {
                CameraController.Instance.ExitProgramMode();
                StartCoroutine(Util.CallbackCoroutine(DialogueManager.Instance.StartDialogue(dialogue),
                    () => gameObject.SetActive(false)));
                _shown = true;
            }
        }
    }
}