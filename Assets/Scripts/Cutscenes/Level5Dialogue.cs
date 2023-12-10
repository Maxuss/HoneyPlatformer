using System.Collections;
using Dialogue;
using Level;
using UnityEngine;

namespace Cutscenes
{
    public class Level5Dialogue: MonoBehaviour, ILevelEntranceCutscene
    {
        [SerializeField]
        private DialogueDefinition dialogue;

        private IEnumerator DialogueStart()
        {
            yield return DialogueManager.Instance.StartDialogue(dialogue);
            ToastManager.Instance.ShowToast("С - режим редактирования соединений. ПКМ по объекту - начать связь, ЛКМ по объекту - закончить.");
        }
        
        public void StartCutscene()
        {
            StartCoroutine(DialogueStart());
        }
    }
}