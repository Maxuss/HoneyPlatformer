using System.Collections;
using Controller;
using Dialogue;
using Level;
using UnityEngine;

namespace Cutscenes
{
    public class Level6Cutscene: MonoBehaviour, ILevelEntranceCutscene
    {
        [SerializeField]
        private DialogueDefinition dialogue;
        
        public void StartCutscene()
        {
            StartCoroutine(Cutscene());
        }

        private IEnumerator Cutscene()
        {
            PlayerController.Instance.IsDisabled = true;
            PlayerController.Instance.InCutscene = true;
            yield return new WaitForSeconds(0.5f);
            yield return DialogueManager.Instance.StartDialogue(dialogue);
            ToastManager.Instance.ShowToast("Генератор щитов отпугивает всех пчёл в комнате");
            PlayerController.Instance.IsDisabled = false;
            PlayerController.Instance.InCutscene = false;
        }
    }
}