using System.Collections;
using Controller;
using Dialogue;
using Level;
using Save;
using UnityEngine;

namespace Cutscenes
{
    public class Level18Cutscene: MonoBehaviour, ILevelEntranceCutscene
    {
        [SerializeField]
        private DialogueDefinition dialogue;
        [SerializeField]
        private AudioClip crickets;
        
        public void StartCutscene()
        {
            StartCoroutine(Cutscene());
        }

        private IEnumerator Cutscene()
        {
            PlayerController.Instance.IsDisabled = true;
            PlayerController.Instance.StillCommitMovement = true;
            PlayerController.Instance.InCutscene = true;
            
            yield return DialogueManager.Instance.StartDialogue(dialogue);
            yield return MusicManager.Instance.Crossfade(crickets, .5f, .25f);
            yield return new WaitForSeconds(1f);
            yield return PlayerController.Instance.FadeIn();
            PlayerController.Instance.ShowTBC();
            
            PlayerController.Instance.IsDisabled = false;
            PlayerController.Instance.InCutscene = false;
            PlayerController.Instance.StillCommitMovement = true;
        }

        
    }
}