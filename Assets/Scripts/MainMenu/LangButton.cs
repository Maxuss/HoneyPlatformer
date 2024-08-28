using System;
using I18N;
using TMPro;
using UnityEngine;
using Utils;

namespace MainMenu
{
    public class LangButton: MonoBehaviour
    {
        [SerializeField]
        private NewMenuManager menu;
        
        private void Start()
        {
            var dd = GetComponent<TMP_Dropdown>();
            dd.value = (int) SettingManager.Instance.ChosenLanguage;
            dd.onValueChanged.AddListener(lang =>
            {
                menu.ChangeLanguage(lang);
                SettingManager.Instance.ChosenLanguage = (Language) lang;
                menu.ReloadSaves();
            });
        }
    }
}