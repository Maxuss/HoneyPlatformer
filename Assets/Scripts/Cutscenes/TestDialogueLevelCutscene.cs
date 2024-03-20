using System.Collections;
using Controller;
using Dialogue;
using Level;
using UnityEngine;

namespace Cutscenes
{
    public class TestDialogueLevelCutscene: MonoBehaviour, ILevelEntranceCutscene
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
            PlayerController.Instance.StillCommitMovement = false;
            PlayerController.Instance.InCutscene = true;

            yield return DialogueManager.Instance.StartDialogue(dialogue);
            
            PlayerController.Instance.IsDisabled = false;
            PlayerController.Instance.StillCommitMovement = true;
            PlayerController.Instance.InCutscene = false;
        }
    }
}