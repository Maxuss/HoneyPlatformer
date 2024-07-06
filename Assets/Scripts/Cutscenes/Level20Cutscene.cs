using System.Collections;
using Controller;
using Dialogue;
using Level;
using NPC;
using Save;
using UnityEngine;
using Utils;

namespace Cutscenes
{
    public class Level20Cutscene: MonoBehaviour, ILevelEntranceCutscene
    {
        [SerializeField]
        private AudioClip crickets;

        [SerializeField]
        private DialogueDefinition dialogue1;

        [SerializeField]
        private DialogueDefinition dialogue2;

        [SerializeField]
        private NpcController mefodiy;
        
        public void StartCutscene()
        {
            StartCoroutine(Cutscene());
        }
        
        private IEnumerator MoveTowards(Vector3 pos)
        {
            var endPos = PlayerController.Instance.transform.position + pos;
            while (Util.SqrDistance(PlayerController.Instance.transform.position, endPos) > .5f &&
                   (endPos - PlayerController.Instance.transform.position).x > 0)
            {
                PlayerController.Instance.Velocity = new Vector2(9f, 0f);
                yield return null;
            }

            PlayerController.Instance.Velocity = Vector2.zero;
        }

        private IEnumerator Cutscene()
        {
            PlayerController.Instance.IsDisabled = true;
            PlayerController.Instance.StillCommitMovement = true;
            PlayerController.Instance.InCutscene = true;

            yield return new WaitForSeconds(1f);

            yield return DialogueManager.Instance.StartDialogue(dialogue1);
            SfxManager.Instance.Play(crickets, .4f);
            yield return new WaitForSeconds(3f);
            yield return DialogueManager.Instance.StartDialogue(dialogue2);
            
            StartCoroutine(MoveTowards(new Vector3(14f, 0f, 0f)));
            StartCoroutine(mefodiy.Walk(14f, .6f));
            
            yield return LevelLoader.Instance.TransitionLevel(25);
            
            PlayerController.Instance.IsDisabled = false;
            PlayerController.Instance.StillCommitMovement = true;
            PlayerController.Instance.InCutscene = false;
        }
    }
}