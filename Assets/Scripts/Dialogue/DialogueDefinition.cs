using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Dialogue
{
    [CreateAssetMenu(fileName = "DialogueDefinition", menuName = "ScriptableObjects/DialogueDefinition", order = 1)]
    public class DialogueDefinition: ScriptableObject
    {
        [FormerlySerializedAs("Speeches")] 
        public List<DialogueSpeech> speeches;
    }

    [Serializable]
    public class DialogueSpeech
    {
        public SpeakingPerson speaker;
        public AudioClip audio;
        public string text;
        public string characterName;
        [Range(0.5f, 3f)]
        public float speedModifier = 1f;
    }

    public enum SpeakingPerson
    {
        Sapsan,
        Mefodiy,
        Sasha,
        Kris,
        Oleg,
        Captain,
        Don
    }
}