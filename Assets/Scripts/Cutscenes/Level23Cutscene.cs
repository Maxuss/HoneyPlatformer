using System.Collections;
using Controller;
using Dialogue;
using Level;
using NPC;
using Objects.Misc;
using UnityEngine;

namespace Cutscenes
{
    public class Level23Cutscene: MonoBehaviour, ILevelEntranceCutscene
    {
        [SerializeField]
        private NpcController sasha;

        [SerializeField]
        private Transform captainPos;

        [SerializeField]
        private Animator entranceDoor;

        [SerializeField]
        private DialogueDefinition dialogue;

        [SerializeField]
        private AudioClip bossMusic;

        [SerializeField] 
        private CountdownDisplay display;
        
        public void StartCutscene()
        {
            StartCoroutine(Cutscene());
        }

        private IEnumerator Cutscene()
        {
            PlayerController.Instance.IsDisabled = true;
            PlayerController.Instance.InCutscene = true;
            
            entranceDoor.Play("UnlockedDoor");
            
            sasha.transform.position -= new Vector3(0f, 100f, 0);

            yield return sasha.Walk(8f);
            entranceDoor.Play("EntranceDoor");

            CameraController.Instance.DisableFollow = true;

            yield return CameraController.Instance.TransitionToPoint(captainPos.position, 3f);
            yield return new WaitForSeconds(.8f);
            yield return DialogueManager.Instance.StartDialogue(dialogue);
            yield return MusicManager.Instance.Crossfade(bossMusic, 1f, .3f, true);
            MusicManager.Instance.dontPlayAmbient = true;

            CameraController.Instance.DisableFollow = false;
            
            PlayerController.Instance.IsDisabled = false;
            PlayerController.Instance.InCutscene = false;
            
            ToastManager.Instance.ShowToast("Покиньте комнату до того как истечет время!");
            display.IsActive = true;
        }
    }
}