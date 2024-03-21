using System.Collections;
using Controller;
using Dialogue;
using Save;
using UnityEngine;
using Utils;

namespace Cutscenes
{
    public class Level16Cutscene: MonoBehaviour, ILevelEntranceCutscene
    {
        [SerializeField]
        private DialogueDefinition dialogue;

        [SerializeField]
        private DialogueDefinition dialogue2;
        
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

            yield return MoveTowards(new Vector3(4f, 0f, 0f));

            yield return DialogueManager.Instance.StartDialogue(dialogue);
            yield return PlayerController.Instance.FadeIn();
            yield return new WaitForSeconds(1f);
            var tf = PlayerController.Instance.gameObject.transform;
            tf.position += new Vector3(8f, 0f, 0f);
            PlayerController.Instance.gameObject.GetComponent<SpriteRenderer>().flipX = true;
            yield return PlayerController.Instance.FadeOut();

            yield return new WaitForSeconds(0.5f);
            yield return DialogueManager.Instance.StartDialogue(dialogue2);
            yield return LevelLoader.Instance.TransitionLevel(17);
            
            PlayerController.Instance.IsDisabled = false;
            PlayerController.Instance.InCutscene = false;
            PlayerController.Instance.StillCommitMovement = true;
        }
    }
}