using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MainMenu
{
    public class SaveBtn: MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField]
        private int idx;
        [SerializeField]
        private NewMenuManager menu;
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            menu.HighlightSave(idx);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            menu.UnhighlightSave(idx);
        }
    }
}