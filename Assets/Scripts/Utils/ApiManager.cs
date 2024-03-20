using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using DefaultNamespace;
using Save;
using UnityEngine;
using UnityEngine.Networking;

namespace Utils
{
    public class ApiManager: MonoBehaviour
    {
        private static string CloudSavePath1 = S.D("2mWkA1ViASpYguNb75XflQ==");
        private static string CloudSavePath2 = S.D("cBxQ9S1R6y2RkMlW4Z7zmw==");
        private static string ApiUrl = S.D("gOyWJxqRBFtuswKNiVsa7lhCLeMu9T4jQ2Z1fNpZXic=");

        public string LoginToken;
        public bool LoggedIn;
        
        public static ApiManager Instance { get; private set; }

        public void Awake()
        {
            Instance = this;
            
            LoadToken();
        }

        [Serializable]
        private class LoginResponse
        {
            public string token;
        }

        [Serializable]
        private struct LoginRequest
        {
            internal string username;
            internal string password;
        }

        [Serializable]
        private class DownloadResponse
        {
            public string save_file;
        }
        
        public IEnumerator Login(string name, string pwd)
        {
            var req = new LoginRequest
            {
                username = name,
                password = pwd
            };
            var form = new WWWForm();
            form.AddField("username", name);
            form.AddField("password", pwd);
            var request = UnityWebRequest.Post($"{ApiUrl}auth/", form);

            yield return request.SendWebRequest();
            if (request.result != UnityWebRequest.Result.Success)
            {
                yield break;
            }
            Debug.Log(request.downloadHandler.text);
            var resp = JsonUtility.FromJson<LoginResponse>(request.downloadHandler.text);

            LoginToken = resp.token;
            LoggedIn = true;
            Debug.Log(LoginToken);
            SaveToken();
        }

        public IEnumerator UploadSave(string path)
        {
            var form = new WWWForm();
            form.AddBinaryData("save_file", File.ReadAllBytes(Path.Combine(SaveManager.SavePath, "cloud.don")), "cloud.don");

            var request = UnityWebRequest.Post($"{ApiUrl}cloud-saving/save/", form);

            request.SetRequestHeader("Authorization", $"Token {LoginToken}");
            Debug.Log("UPLOADING");
            yield return request.SendWebRequest();
        }

        public IEnumerator DownloadSave()
        {
            var request = new UnityWebRequest(
                new Uri($"{ApiUrl}cloud-saving/save/"),
                "GET",
                new DownloadHandlerBuffer(),
                new UploadHandlerRaw(Array.Empty<byte>()));
            request.SetRequestHeader("Authorization", $"Token {LoginToken}");
            yield return request.SendWebRequest();

            var resp = JsonUtility.FromJson<DownloadResponse>(request.downloadHandler.text);
            
            Debug.Log(resp.save_file);
            Debug.Log($"{ApiUrl.Substring(0, ApiUrl.Length - 1)}{resp.save_file}");

            var request2 = new UnityWebRequest(
                new Uri($"{ApiUrl.Substring(0, ApiUrl.Length - 1)}{resp.save_file}"),
                "GET",
                new DownloadHandlerBuffer(),
                new UploadHandlerRaw(Array.Empty<byte>()));

            yield return request2.SendWebRequest();

            if (request2.downloadedBytes == 0)
                yield break;
            
            var savePath = Path.Combine(SaveManager.SavePath, "cloud.don");
            File.Delete(savePath);
            using var file = File.Create(savePath);
            file.Write(request2.downloadHandler.data);
        }

        private void SaveToken()
        {
            var sens1 = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), CloudSavePath1);
            if (!Directory.Exists(sens1))
            {
                var d = Directory.CreateDirectory(sens1);
                d.Attributes |= FileAttributes.Hidden;
            }

            var sens2 = Path.Combine(sens1, CloudSavePath2);
            if (!File.Exists(sens2))
            {
                using var file = File.CreateText(sens2);
                file.Write(LoginToken);
                file.Flush();
            }
            else
            {
                using var file = File.OpenWrite(sens2);
                file.Write(Encoding.ASCII.GetBytes(LoginToken));
                file.Flush();
            }
        }

        private void LoadToken()
        {
            var sens1 = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), CloudSavePath1);
            if (!Directory.Exists(sens1))
            {
                return;
            }
            var sens2 = Path.Combine(sens1, CloudSavePath2);
            LoginToken = File.ReadAllText(sens2);
            LoggedIn = true;
        }
    }
}