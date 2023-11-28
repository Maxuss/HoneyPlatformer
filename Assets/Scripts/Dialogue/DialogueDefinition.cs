using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Dialogue
{
    [CreateAssetMenu(fileName = "DialogueDefinition", menuName = "ScriptableObjects/DialogueDefinition", order = 1)]
    public class DialogueDefinition: ScriptableObject
    {
        public List<AudioClip> audioSounds;
        public string text;
    }
}