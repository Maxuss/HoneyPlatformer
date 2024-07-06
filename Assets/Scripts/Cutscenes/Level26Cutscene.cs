using System.Collections;
using Controller;
using Dialogue;
using Level;
using NPC;
using UnityEngine;

namespace Cutscenes
{
    public class Level26Cutscene: MonoBehaviour, ILevelEntranceCutscene
    {
        [SerializeField]
        private NpcController captain;

        [SerializeField]
        private NpcController kris;

        [SerializeField]
        private Animator entranceDoor;

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
            MusicManager.Instance.dontPlayAmbient = false;

            MusicManager.Instance.NextAmbientTrack();

            entranceDoor.Play("UnlockedDoor");
            
            kris.transform.position -= new Vector3(0f, 100f, 0);

            yield return kris.Walk(8f);
            entranceDoor.Play("EntranceDoor");

            yield return new WaitForSeconds(1f);

            yield return DialogueManager.Instance.StartDialogue(dialogue);

            yield return new WaitForSeconds(2f);

            yield return PlayerController.Instance.FadeIn();

            PlayerController.Instance.ShowTBC();
        }
    }
}