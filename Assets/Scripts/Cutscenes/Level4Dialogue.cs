using System.Collections;
using Dialogue;
using Level;
using UnityEngine;

namespace Cutscenes
{
    public class Level4Dialogue: MonoBehaviour, ILevelEntranceCutscene
    {
        [SerializeField]
        private DialogueDefinition dialogue;

        private IEnumerator DialogueStart()
        {
            yield return DialogueManager.Instance.StartDialogue(dialogue);
            ToastManager.Instance.ShowToast("Иногда бывает полезно заранее осмотреть уровень в режиме программирования.");
        }
        
        public void StartCutscene()
        {
            StartCoroutine(DialogueStart());
        }
    }
}