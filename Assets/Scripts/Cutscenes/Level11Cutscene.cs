using System.Collections;
using Controller;
using DG.Tweening;
using Dialogue;
using Level;
using NPC;
using Objects;
using Save;
using UnityEngine;
using Utils;

namespace Cutscenes
{
    public class Level11Cutscene: MonoBehaviour, ILevelEntranceCutscene
    {
        [SerializeField]
        private NpcController sasha;
        
        [SerializeField]
        private SpriteRenderer darkLeft;
        [SerializeField]
        private SpriteRenderer darkRight;

        [SerializeField]
        private ElevatorDoor doorLeft;
        [SerializeField]
        private ElevatorDoor doorRight;

        [SerializeField]
        private DialogueDefinition dialogue1;
        [SerializeField]
        private DialogueDefinition dialogue2;
        [SerializeField]
        private DialogueDefinition dialogue3;

        [SerializeField]
        private AudioClip elevatorMusic;
        [SerializeField]
        private AudioClip alarm;

        [SerializeField]
        private Transform gigaArm;
        
        public void StartCutscene()
        {
            StartCoroutine(Cutscene());
        }
        
        private IEnumerator MoveTowards(Vector3 pos)
        {
            var endPos = PlayerController.Instance.transform.position + pos;
            while (Util.SqrDistance(PlayerController.Instance.transform.position, endPos) > .5f && (endPos - PlayerController.Instance.transform.position).x > 0)
            {
                PlayerController.Instance.Velocity = new Vector2(9f, 0f);
                yield return null;
            }
            PlayerController.Instance.Velocity = Vector2.zero;
        }

        private IEnumerator Cutscene()
        {
            // PlayerController.Instance.Velocity = Vector2.zero;
            PlayerController.Instance.IsDisabled = true;
            PlayerController.Instance.StillCommitMovement = true;
            PlayerController.Instance.InCutscene = true;
            
            doorLeft.Open();

            StartCoroutine(this.CallbackCoroutine(sasha.Walk(9f, .5f), () => sasha.GetComponent<SpriteRenderer>().flipX = true));
            yield return MoveTowards(new Vector3(14f, 0, 0));
            
            doorLeft.Close();

            var leftOpacity = 0f;
            while (leftOpacity < 1f)
            {
                leftOpacity += 0.5f * Time.deltaTime;
                darkLeft.color = new Color(0, 0, 0, leftOpacity);
                yield return null;
            }

            darkLeft.color = new Color(0, 0, 0, 1f);

            StartCoroutine(MusicManager.Instance.Crossfade(elevatorMusic, 0.5f, 0.3f));

            yield return new WaitForSeconds(5f);

            yield return DialogueManager.Instance.StartDialogue(dialogue1);
            
            doorRight.Open();
            
            // TODO: scary music or something
            gigaArm.DOMoveX(267.75f, 0.5f);

            yield return MusicManager.Instance.Crossfade(alarm, 0.5f, 0.4f);
            
            yield return new WaitForSeconds(1f);
            
            yield return DialogueManager.Instance.StartDialogue(dialogue2);

            MusicManager.Instance.NextAmbientTrack();
            yield return new WaitForSeconds(1.6f);

            gigaArm.DOMoveX(275f, 1f);
            yield return new WaitForSeconds(1f);
            gigaArm.gameObject.SetActive(false);
            
            var rightOpacity = 1f;
            while (rightOpacity > 0f)
            {
                rightOpacity -= 0.6f * Time.deltaTime;
                darkRight.color = new Color(0, 0, 0, rightOpacity);
                yield return null;
            }
            darkRight.color = new Color(0f, 0f, 0f, 0f);

            StartCoroutine(this.CallbackCoroutine(sasha.Walk(7f, .4f), () => sasha.GetComponent<SpriteRenderer>().flipX = true));
            yield return MoveTowards(new Vector3(8f, 0f, 0f));

            yield return DialogueManager.Instance.StartDialogue(dialogue3);
            
            yield return LevelLoader.Instance.TransitionLevel(13);
            
            PlayerController.Instance.IsDisabled = false;
            PlayerController.Instance.InCutscene = false;
            PlayerController.Instance.StillCommitMovement = true;
        }
    }
}