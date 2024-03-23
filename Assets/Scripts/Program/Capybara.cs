using System;
using Dialogue;
using UnityEngine;
using UnityEngine.UIElements;

namespace Program
{
    public class Capybara: MonoBehaviour
    {
        [SerializeField] private DialogueDefinition dialogue;

        private void OnMouseOver()
        {
            if (Input.GetMouseButton((int)MouseButton.LeftMouse) || Input.GetMouseButton((int) MouseButton.RightMouse))
            {
                StartCoroutine(DialogueManager.Instance.StartDialogue(dialogue));
            }
        }
    }
}