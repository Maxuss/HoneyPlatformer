using System.Collections;
using Controller;
using Dialogue;
using NPC;
using Save;
using UnityEngine;
using Utils;

namespace Cutscenes
{
    public class Level10Cutscene: MonoBehaviour, ILevelEntranceCutscene
    {
        [SerializeField]
        private DialogueDefinition dialogue1;
        [SerializeField]
        private DialogueDefinition dialogue2;

        [SerializeField]
        private NpcController sasha;
        [SerializeField]
        private NpcController mefodiy;
        [SerializeField]
        private Animator entranceDoor;
        [SerializeField]
        private Sprite openDoorSprite;
        private IEnumerator MoveTowards(Vector3 pos)
        {
            var endPos = PlayerController.Instance.transform.position + pos;
            while (Util.SqrDistance(PlayerController.Instance.transform.position, endPos) > .5f)
            {
                PlayerController.Instance.Velocity = new Vector2(7f, 0f);
                yield return null;
            }
        }
        
        public void StartCutscene()
        {
            StartCoroutine(Cutscene());
        }

        private IEnumerator Cutscene()
        {
            PlayerController.Instance.IsDisabled = true;
            PlayerController.Instance.StillCommitMovement = true;
            PlayerController.Instance.InCutscene = true;

            StartCoroutine(MoveTowards(new Vector3(4f, 0, 0)));
            yield return sasha.Walk(-2f, .5f);
            yield return new WaitForSeconds(1f);
            yield return DialogueManager.Instance.StartDialogue(dialogue1);
            yield return new WaitForSeconds(1f);
            
            yield return new WaitForSeconds(.5f);
            yield return DialogueManager.Instance.StartDialogue(dialogue2);

            yield return new WaitForSeconds(.5f);

            StartCoroutine(MoveTowards(new Vector3(5f, 0f, 0f)));
            StartCoroutine(mefodiy.Walk(5f, .4f));
            StartCoroutine(sasha.Walk(5f, .3f));
            
            yield return LevelLoader.Instance.TransitionLevel(12);
            
            PlayerController.Instance.IsDisabled = false;
            PlayerController.Instance.InCutscene = false;
            PlayerController.Instance.StillCommitMovement = true;
        }
    }
}