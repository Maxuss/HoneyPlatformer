using System.Collections;
using Controller;
using Dialogue;
using Save;
using UnityEngine;

namespace Cutscenes
{
    public class Level21Cutscene: MonoBehaviour, ILevelEntranceCutscene
    {
        [SerializeField]
        private DialogueDefinition dialogue1;

        [SerializeField]
        private DialogueDefinition dialogue2;
        
        [SerializeField]
        private DialogueDefinition dialogue3;

        [SerializeField]
        private DialogueDefinition dialogue4;

        [SerializeField]
        private Transform donPos;

        
        public void StartCutscene()
        {
            StartCoroutine(Cutscene());
        }

        private IEnumerator Cutscene()
        {
            PlayerController.Instance.IsDisabled = true;
            PlayerController.Instance.InCutscene = true;
            
            yield return new WaitForSeconds(2f);

            yield return DialogueManager.Instance.StartDialogue(dialogue1);

            yield return new WaitForSeconds(0.5f);
            yield return DialogueManager.Instance.StartDialogue(dialogue2);

            yield return new WaitForSeconds(1f);
            CameraController.Instance.DisableFollow = true;
            yield return CameraController.Instance.TransitionToPoint(donPos.position, 2f);

            yield return new WaitForSeconds(1.5f);
            
            yield return DialogueManager.Instance.StartDialogue(dialogue3);

            yield return new WaitForSeconds(1.5f);
            CameraController.Instance.DisableFollow = false;

            yield return new WaitForSeconds(1f);
            
            yield return DialogueManager.Instance.StartDialogue(dialogue4);
            
            yield return LevelLoader.Instance.TransitionLevel(26);

            PlayerController.Instance.IsDisabled = false;
            PlayerController.Instance.InCutscene = false;
        }
    }
}