using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Dialogue
{
    [CreateAssetMenu(fileName = "DialogueDefinition", menuName = "ScriptableObjects/DialogueDefinition", order = 1)]
    public class DialogueDefinition: ScriptableObject
    {
        public Sprite[] sprites;
        public AudioClip[] audioSounds;
        public string text;
        public string characterName;
        public float speedModifier = 1f;
    }
}