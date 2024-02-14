using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Program.UI
{
    public class TerminalCallbackButton: MonoBehaviour
    {
        public System.Action ClickHandler;
        
        public void OnClick()
        {
            ClickHandler?.Invoke();
        }
    }
}