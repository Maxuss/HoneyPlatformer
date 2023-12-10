using System.Collections;
using Dialogue;
using Level;
using UnityEngine;

namespace Cutscenes
{
    public class Level6Dialogue: MonoBehaviour, ILevelEntranceCutscene
    {
        [SerializeField]
        private DialogueDefinition dialogue;

        private IEnumerator DialogueStart()
        {
            yield return DialogueManager.Instance.StartDialogue(dialogue);
            ToastManager.Instance.ShowToast("Такие объекты как модификатор имеют слоты для ввода и вывода данных.");
        }
        
        public void StartCutscene()
        {
            StartCoroutine(DialogueStart());
        }
    }
}