using System;
using TMPro;
using UnityEngine;

namespace I18N
{
    [RequireComponent(typeof(TMP_Text))]
    public class LocalizedText: MonoBehaviour
    {
        [SerializeField]
        private string str;
        
        private string replacement = "";

        public void UpdateText(string newStr, object repl = null)
        {
            str = newStr;
            var tmp = GetComponent<TMP_Text>();
            replacement = repl?.ToString() ?? "null";
            tmp.text = LocalizationManager.Instance.Translated(str, replacement);
        }
        
        public void Start()
        {
            var tmp = GetComponent<TMP_Text>();
            str = tmp.text.StartsWith("#") ? tmp.text.Substring(1) : str;
            LocalizationManager.OnLanguageChange += _ =>
            {
                tmp.text = LocalizationManager.Instance.Translated(str, replacement);
            };
            tmp.text = LocalizationManager.Instance.Translated(str, replacement);
        }
    }
}