using System.Collections.Generic;
using System.IO;
using System.Linq;
using Dialogue;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using DialogueDefinition = Dialogue.DialogueDefinition;

namespace I18N
{
    #if UNITY_EDITOR
    [CreateAssetMenu(fileName = "ScriptableObjects", menuName = "Localization Metafile")]
    public class LocalizationScript: ScriptableObject
    {
        public string localizationCode;
    }
    
    public class CreateAssetBundles
    {
        [MenuItem("Assets/Build Bundles")]
        static void BuildBundles()
        {
            BuildPipeline.BuildAssetBundles(new BuildAssetBundlesParameters
            {
                outputPath = "Assets/AssetBundles", options = BuildAssetBundleOptions.None,
            });
        }
    }
    
    [CustomEditor(typeof(LocalizationScript))]
    public class TestScriptableEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var script = (LocalizationScript) target;

            if(GUILayout.Button("Bake localization JSON", GUILayout.Height(20)))
            {
                ProcessFiles(script.localizationCode);
            }

            if (GUILayout.Button("Bake dialogue objects", GUILayout.Height(20)))
            {
                BakeDialogues();
            }
        }

        private void BakeDialogues()
        {
            var associated = AssetDatabase.FindAssets("t:DialogueDefinition");
            foreach (var (path, dialogue) in associated.Select(AssetDatabase.GUIDToAssetPath)                
                         .Select(it => (it, (DialogueDefinition)AssetDatabase.LoadAssetAtPath(it, typeof(DialogueDefinition)))))
            {
                Undo.RecordObject(dialogue, $"Changed dialogue data on {path}");
                dialogue.speeches = dialogue.speeches.Select((it, idx) =>
                {
                    var k = $"{path.Replace("Assets/Dialogues/", "").Replace(".asset", "")}.{idx}";
                    return new DialogueSpeech
                    {
                        audio = it.audio,
                        characterName = it.characterName,
                        speaker = it.speaker,
                        speedModifier = it.speedModifier,
                        text = k
                    };
                }).ToList();
                EditorUtility.SetDirty(dialogue);
            }
            
            AssetDatabase.SaveAssets();
        }

        private void ProcessFiles(string lang)
        {
            if (!AssetDatabase.IsValidFolder("Assets/Dialogues/Localized"))
                AssetDatabase.CreateFolder("Assets/Dialogues", "Localized");
            var dict = new Dictionary<string, string>();
            
            if (File.Exists(Application.dataPath + $"/Dialogues/Localized/localized_{lang}.json"))
            {
                var txt = File.ReadAllText(Application.dataPath + $"/Dialogues/Localized/localized_{lang}.json");
                var deserialized = JsonConvert.DeserializeObject<Dictionary<string, string>>(txt);
                foreach (var pair in deserialized)
                {
                    dict.Add(pair.Key, pair.Value);
                }
            }
            
            var associated = AssetDatabase.FindAssets("t:DialogueDefinition");
                    
            var af = associated.Select(AssetDatabase.GUIDToAssetPath)
                .Select(it => (it, (DialogueDefinition)AssetDatabase.LoadAssetAtPath(it, typeof(DialogueDefinition))))
                .Select(it => (it.Item1, it.Item2.speeches));
            
            foreach (var (filePath, speeches) in af)
            {
                Debug.Log($"PROCESSING {filePath} {speeches.Count}");
                for (var i = 0; i < speeches.Count; i++)
                {
                    var k = $"{filePath.Replace("Assets/Dialogues/", "").Replace(".asset", "")}.{i}";
                    if (dict.ContainsKey(k))
                        continue;
                    dict[k] = $"{speeches[i].characterName}/{speeches[i].text}";
                }
            }

            Debug.Log(dict.Count);
            
            var json = JsonConvert.SerializeObject(dict, Formatting.Indented);

            var file = File.CreateText($"{Application.dataPath}/Dialogues/Localized/localized_{lang}.json");
            file.Write(json);
            file.Flush();
            file.Close();
            AssetDatabase.SaveAssets();
        }
    }
    #endif
}