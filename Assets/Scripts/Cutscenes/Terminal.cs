using Controller;
using Level;
using Objects;
using TMPro;
using UnityEngine;

namespace Cutscenes
{
    public class Terminal: MonoBehaviour, IInteractable
    {
        [SerializeField]
        private string text;

        public void OnInteract()
        {
            PlayerController.Instance.IsDisabled = true;
            TerminalManager.Instance.OpenTerminal(text);
        }
    }
}