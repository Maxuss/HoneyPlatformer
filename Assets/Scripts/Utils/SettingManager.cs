using System;
using System.IO;
using Save;
using UnityEngine;

namespace Utils
{
    public class SettingManager: MonoBehaviour
    {
        public static SettingManager Instance { get; private set; }
        
        public float MusicVolume = 1f;
        public float SfxVolume = 1f;

        private void Awake()
        {
            DontDestroyOnLoad(this);
            Instance = this;
            var path = Path.Join(SaveManager.SavePath, "pref.json");
            if (!File.Exists(path))
                return;
            
            var text = File.ReadAllText(path);
            var obj = JsonUtility.FromJson<PrefObject>(text);
            MusicVolume = obj.MusicVolume;
            SfxVolume = obj.SfxVolume;
        }

        public void Save()
        {
            var path = Path.Join(SaveManager.SavePath, "pref.json");
            using var file = new StreamWriter(File.Exists(path) ? File.OpenWrite(path) : File.Create(path));
            file.Write(JsonUtility.ToJson(this, true));
            file.Close();
        }

        [Serializable]
        public struct PrefObject
        {
            public float MusicVolume;
            public float SfxVolume;
        }
    }
}