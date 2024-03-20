using System.Collections;
using Controller;
using Dialogue;
using Level;
using NPC;
using UnityEngine;
using Utils;

namespace Cutscenes
{
    public class Level8Cutscene: MonoBehaviour, ILevelEntranceCutscene
    {
        [SerializeField]
        private NpcController mefodiy;

        [SerializeField]
        private DialogueDefinition dialogue1;
        [SerializeField]
        private DialogueDefinition dialogue2;
        
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
            
            StartCoroutine(MoveTowards(new Vector2(13, 0)));
            StartCoroutine(mefodiy.Walk(13, .45f));

            yield return DialogueManager.Instance.StartDialogue(dialogue1);

            yield return new WaitForSeconds(.5f);

            StartCoroutine(mefodiy.Walk(-25f, .7f));

            yield return new WaitForSeconds(1.5f);

            yield return DialogueManager.Instance.StartDialogue(dialogue2);
            
            ToastManager.Instance.ShowToast("B - позвонить Дону");
            
            PlayerController.Instance.IsDisabled = false;
            PlayerController.Instance.StillCommitMovement = true;
            PlayerController.Instance.InCutscene = false;
        }
    }
}