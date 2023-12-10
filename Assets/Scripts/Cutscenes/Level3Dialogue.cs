using System.Collections;
using Dialogue;
using Level;
using UnityEngine;

namespace Cutscenes
{
    public class Level3Dialogue: MonoBehaviour, ILevelEntranceCutscene
    {
        [SerializeField]
        private DialogueDefinition dialogue;

        private IEnumerator DialogueStart()
        {
            yield return DialogueManager.Instance.StartDialogue(dialogue);
            ToastManager.Instance.ShowToast("V - перейти в режим программирования. Затем нажмите по объекту чтобы редактировать.");
        }
        
        public void StartCutscene()
        {
            StartCoroutine(DialogueStart());
        }
    }
}