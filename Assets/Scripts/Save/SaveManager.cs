using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using Utils;

namespace Save
{
    public static class SaveManager
    {
        public static SaveState CurrentState;

        public static string SavePath =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "BRZ");
        
        private static void CheckSaveDir()
        {
            try
            {
                if (!Directory.Exists(SavePath))
                    Directory.CreateDirectory(SavePath);
            }
            catch (ArgumentNullException e)
            {
                
            }
        }

        public static Dictionary<int, SaveState?> AllSaves()
        {
            return new[] { 0, 1, 2, 3 }.Select(each => (each, LoadGame(each))).ToDictionary(x => x.each, x => x.Item2);
        }

        public static void SaveGame(bool auto = false)
        {
            CheckSaveDir();
            var path = Path.Combine(SavePath, "game" + (auto ? "_auto" : CurrentState.SaveIndex) + ".don");
            
            using var fileStream = new StreamWriter(File.Create(path));

            fileStream.Write(JsonUtility.ToJson(CurrentState, true));
            fileStream.Flush();
            fileStream.Close();
        }

        public static SaveState? LoadGame(int saveIdx)
        {
            CheckSaveDir();
            var path = Path.Combine(SavePath, "game" + (saveIdx == 3 ? "_auto" : saveIdx) + ".don");
            if (!File.Exists(path))
                return null;
            
            using var fileStream = File.OpenRead(path);
            
            using var reader = new StreamReader(fileStream);
            var save = JsonUtility.FromJson<SaveState>(reader.ReadToEnd());
            return save;
        }

        public static SaveState? LoadCloud()
        {
            throw new NotImplementedException("Removed feature");
            CheckSaveDir();
            var path = Path.Combine(SavePath, $"cloud.don");
            if (!File.Exists(path))
                return null;
            
            using var fileStream = File.OpenRead(path);
            
            var binaryFormatter = new BinaryFormatter();
            var save = (SaveState) binaryFormatter.Deserialize(fileStream);
            return save;
        }

        public static bool HasCloudSave()
        {
            return File.Exists(Path.Combine(SavePath, $"cloud.don"));
        }

        public static IEnumerator ReplaceWithCloud(int toReplace)
        {
            yield return ApiManager.Instance.DownloadSave();
            File.Delete(Path.Combine(SavePath, $"game{toReplace}.don"));
            File.Copy(Path.Combine(SavePath, "cloud.don"), Path.Combine(SavePath, $"game{toReplace}.don"));
        }

        public static IEnumerator UploadToCloud(int toUpload)
        {
            File.Delete(Path.Combine(SavePath, "cloud.don"));
            File.Copy(Path.Combine(SavePath, $"game{toUpload}.don"), Path.Combine(SavePath, "cloud.don"));
            yield return ApiManager.Instance.UploadSave(Path.Combine(SavePath, $"game{toUpload}.don"));
        }
    }
}