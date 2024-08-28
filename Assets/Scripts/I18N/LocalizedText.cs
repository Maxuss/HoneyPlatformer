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
        
        public void Start()
        {
            var tmp = GetComponent<TMP_Text>();
            var translateString = tmp.text.StartsWith("#") ? tmp.text.Substring(1) : str;
            LocalizationManager.OnLanguageChange += _ =>
            {
                tmp.text = LocalizationManager.Instance.Translated(translateString);
            };
            tmp.text = LocalizationManager.Instance.Translated(translateString);
        }
    }
}