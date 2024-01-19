using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Program.UI
{
    public class TerminalCallbackButton: MonoBehaviour
    {
        public System.Action ClickHandler;

        public void OnMouseEnter()
        {
            Debug.Log("MOUSE ENTERED");
        }

        public void OnClick()
        {
            Debug.Log("CLICKED ME");
            ClickHandler?.Invoke();
        }
    }
}