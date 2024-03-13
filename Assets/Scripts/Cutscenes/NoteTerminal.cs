using Controller;
using Level;
using Objects;
using TMPro;
using UnityEngine;

namespace Cutscenes
{
    public class NoteTerminal: MonoBehaviour, IInteractable
    {
        [SerializeField]
        [TextArea]
        private string text;

        public void OnInteract()
        {
            PlayerController.Instance.IsDisabled = true;
            TerminalManager.Instance.OpenTerminal(text);
        }
    }
}