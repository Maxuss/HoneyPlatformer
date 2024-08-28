using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using Utils;

namespace I18N
{
    public class LocalizationManager: MonoBehaviour
    {
        private Dictionary<string, string> _localization;
        private Language _currentLanguage;
        
        public static LocalizationManager Instance { get; private set; }

        public void Awake()
        {
            DontDestroyOnLoad(this);
            LoadLocalization(SettingManager.Instance.ChosenLanguage);
            Instance = this;
        }

        public void LoadLocalization(Language lang)
        {
            _currentLanguage = lang;
            var dialogues = (TextAsset) AssetDatabase.LoadAssetAtPath($"Assets/Dialogues/Localized/localized_{lang}.json", typeof(TextAsset));
            var extra = (TextAsset) AssetDatabase.LoadAssetAtPath($"Assets/Dialogues/Localized/extra_{lang}.json", typeof(TextAsset));
            Debug.Log(extra.text);

            var dg = JsonConvert.DeserializeObject<Dictionary<string, string>>(dialogues.text);

            var extr = JsonConvert.DeserializeObject<Dictionary<string, string>>(extra.text);
            Debug.Log(extr.Count);
            foreach (var kv in extr)
            {
                
                dg.Add(kv.Key, kv.Value);
            }

            _localization = dg;
        }

        public string Translated(string str)
        {
            if (!_localization.ContainsKey(str))
            {
                Debug.LogWarning($"No translation string for '{str}' in language '{_currentLanguage}'");
                return str;
            }
            return _localization[str];
        }
    }

    public enum Language
    {
        ru_RU,
        en_US
    }
}