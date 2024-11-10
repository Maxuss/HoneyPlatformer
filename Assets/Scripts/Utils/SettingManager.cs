using System;
using System.IO;
using I18N;
using Save;
using UnityEngine;

namespace Utils
{
    public class SettingManager: MonoBehaviour
    {
        public static SettingManager Instance { get; private set; }
        
        public float MusicVolume = 1f;
        public float SfxVolume = 1f;
        public Language ChosenLanguage = Language.ru_RU;
        public bool EnablePixels = false;

        private void Awake()
        {
            
            DontDestroyOnLoad(this);
            Instance = this;
            var path = Path.Join(SaveManager.SavePath, "pref.json");
            if (!File.Exists(path))
            {
                var lg = Application.systemLanguage;
                ChosenLanguage = lg is SystemLanguage.Russian or SystemLanguage.Belarusian or SystemLanguage.Ukrainian ?
                    Language.ru_RU :
                    Language.en_US;
                return;
            }
            
            var text = File.ReadAllText(path);
            var obj = JsonUtility.FromJson<PrefObject>(text);
            MusicVolume = obj.MusicVolume;
            SfxVolume = obj.SfxVolume;
            ChosenLanguage = obj.ChosenLanguage;
            EnablePixels = obj.EnablePixels;
        }

        public void Save()
        {
            var path = Path.Join(SaveManager.SavePath, "pref.json");
            using var file = new StreamWriter(File.Create(path));
            file.Write(JsonUtility.ToJson(this, true));
            file.Close();
        }

        [Serializable]
        public struct PrefObject
        {
            public float MusicVolume;
            public float SfxVolume;
            public Language ChosenLanguage;
            public bool EnablePixels;
        }
    }
}