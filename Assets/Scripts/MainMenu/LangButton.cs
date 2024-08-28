using System;
using TMPro;
using UnityEngine;

namespace MainMenu
{
    public class LangButton: MonoBehaviour
    {
        [SerializeField]
        private NewMenuManager menu;
        
        private void Start()
        {
            var dd = GetComponent<TMP_Dropdown>();
            dd.onValueChanged.AddListener(menu.ChangeLanguage);
        }
    }
}