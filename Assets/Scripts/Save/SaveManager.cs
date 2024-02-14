using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Save
{
    public static class SaveManager
    {
        public static SaveState CurrentState;

        private static string SavePath =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "BRZ");
        
        private static void CheckSaveDir()
        {
            if (!Directory.Exists(SavePath))
                Directory.CreateDirectory(SavePath);
        }

        public static void SaveGame()
        {
            CheckSaveDir();
            var path = Path.Combine(SavePath, $"game{CurrentState.SaveIndex}.don");
            using var fileStream = File.Exists(path) ? File.OpenWrite(path) : File.Create(path);
            
            var binaryFormatter = new BinaryFormatter();
            binaryFormatter.Serialize(fileStream, CurrentState);
        }

        public static SaveState? LoadGame(int saveIdx)
        {
            CheckSaveDir();
            var path = Path.Combine(SavePath, $"game{saveIdx}.don");
            if (!File.Exists(path))
                return null;
            
            using var fileStream = File.OpenRead(path);
            
            var binaryFormatter = new BinaryFormatter();
            var save = (SaveState) binaryFormatter.Deserialize(fileStream);
            CurrentState = save;
            return CurrentState;
        }
    }
}