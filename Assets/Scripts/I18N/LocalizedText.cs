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
            tmp.text = LocalizationManager.Instance.Translated(tmp.text.StartsWith("#") ? tmp.text.Substring(1) : str);
        }
    }
}