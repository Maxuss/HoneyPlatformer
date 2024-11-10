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

        public static event Action<Language> OnLanguageChange;
        
        public static LocalizationManager Instance { get; private set; }

        public void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(this);
            LoadLocalization(SettingManager.Instance?.ChosenLanguage ?? Language.en_US);
        }

        public void LoadLocalization(Language lang)
        {
            _currentLanguage = lang;
            var dialogues = Resources.Load<TextAsset>($"Localized/localized_{lang}");
            var extra = Resources.Load<TextAsset>($"Localized/extra_{lang}");

            var dg = JsonConvert.DeserializeObject<Dictionary<string, string>>(dialogues.text);

            var extr = JsonConvert.DeserializeObject<Dictionary<string, string>>(extra.text);
            Debug.Log(extr.Count);
            foreach (var kv in extr)
            {
                
                dg.Add(kv.Key, kv.Value);
            }

            _localization = dg;
            OnLanguageChange?.Invoke(lang);
        }

        public string Translated(string str, string replacement = null)
        {
            if (!_localization.ContainsKey(str))
            {
                Debug.LogWarning($"No translation string for '{str}' in language '{_currentLanguage}'");
                return str;
            }
            return _localization[str].Replace("{0}", replacement);
        }
    }

    public enum Language
    {
        ru_RU,
        en_US
    }
}