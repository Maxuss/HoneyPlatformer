using System.Collections;
using Controller;
using Dialogue;
using UnityEngine;

namespace Cutscenes
{
    public class Level22Cutscene: MonoBehaviour, ILevelEntranceCutscene
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
            
            yield return new WaitForSeconds(1f);

            yield return DialogueManager.Instance.StartDialogue(dialogue);
            
            PlayerController.Instance.IsDisabled = false;
            PlayerController.Instance.InCutscene = false;
        }
    }
}