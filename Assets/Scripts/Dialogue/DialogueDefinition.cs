using System.Collections.Generic;
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