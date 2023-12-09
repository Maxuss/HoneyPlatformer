using System.Collections;
using Controller;
using Dialogue;
using Level;
using UnityEngine;

namespace Cutscenes
{
    public class Level2Dialogue: MonoBehaviour, ILevelEntranceCutscene
    {
        [SerializeField]
        private DialogueDefinition dialogue;
        
        private IEnumerator DialogueStart()
        {
            yield return DialogueManager.Instance.StartDialogue(dialogue);
            ToastManager.Instance.ShowToast("Удерживать SHIFT - тащить объект за собой. Или толкай его!");
        }

        public void StartCutscene()
        {
            StartCoroutine(DialogueStart());
        }
    }
}