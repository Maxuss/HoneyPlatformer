using System.Collections;
using System.Numerics;
using Controller;
using Dialogue;
using NPC;
using Save;
using UnityEngine;
using Utils;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace Cutscenes
{
    public class Level7Cutscene: MonoBehaviour, ILevelEntranceCutscene
    {
        [SerializeField]
        private DialogueDefinition dialogue1;
        [SerializeField]
        private DialogueDefinition dialogue2;
        [SerializeField]
        private NpcController mefodiy;
        [SerializeField]
        private GameObject disk;
        
        public void StartCutscene()
        {
            StartCoroutine(Cutscene());
        }

        private IEnumerator MoveTowards(Vector3 pos)
        {
            var endPos = PlayerController.Instance.transform.position + pos;
            while (Util.SqrDistance(PlayerController.Instance.transform.position, endPos) > .5f)
            {
                PlayerController.Instance.Velocity = new Vector2(7f, 0f);
                yield return null;
            }
        }
        
        private IEnumerator Cutscene()
        {
            PlayerController.Instance.IsDisabled = true;
            PlayerController.Instance.StillCommitMovement = true;
            PlayerController.Instance.InCutscene = true;

            StartCoroutine(MoveTowards(new Vector2(8, 0)));
            yield return mefodiy.Walk(11, .4f);
            mefodiy.GetComponent<SpriteRenderer>().flipX = true;
            yield return new WaitForSeconds(.5f);
            yield return DialogueManager.Instance.StartDialogue(dialogue1);
            yield return new WaitForSeconds(.5f);
            // TODO: item pick up sound
            disk.SetActive(false);
            yield return new WaitForSeconds(.5f);
            mefodiy.GetComponent<SpriteRenderer>().flipX = false;
            yield return DialogueManager.Instance.StartDialogue(dialogue2);
            StartCoroutine(MoveTowards(new Vector2(4, 0)));
            StartCoroutine(mefodiy.Walk(6, .4f));
            
            yield return LevelLoader.Instance.TransitionLevel(8);
            
            PlayerController.Instance.IsDisabled = false;
            PlayerController.Instance.InCutscene = false;
        }
    }
}