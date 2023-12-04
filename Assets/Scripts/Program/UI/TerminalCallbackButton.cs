using UnityEngine;
using UnityEngine.EventSystems;

namespace Program.UI
{
    public class TerminalCallbackButton: MonoBehaviour, IPointerClickHandler
    {
        public System.Action ClickHandler;
        
        public void OnPointerClick(PointerEventData eventData)
        {
            ClickHandler?.Invoke();
        }
    }
}