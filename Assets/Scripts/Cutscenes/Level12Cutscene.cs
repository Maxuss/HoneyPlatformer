using System.Collections;
using Controller;
using DG.Tweening;
using Dialogue;
using Level;
using NPC;
using Save;
using UnityEngine;
using Utils;

namespace Cutscenes
{
    public class Level12Cutscene: MonoBehaviour, ILevelEntranceCutscene
    {
        [SerializeField]
        private NpcController sasha;
        
        [SerializeField]
        private DialogueDefinition dialogue1;
        [SerializeField]
        private DialogueDefinition dialogue2;

        [SerializeField]
        private AudioClip olegAppearance;

        [SerializeField]
        private GameObject bee1;
        [SerializeField]
        private GameObject bee2;
        
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

            StartCoroutine(MoveTowards(new Vector3(8f, 0f, 0f)));
            yield return sasha.Walk(6f, .6f);

            SfxManager.Instance.Play(olegAppearance);
            yield return new WaitForSeconds(3f);
            yield return DialogueManager.Instance.StartDialogue(dialogue1);

            bee1.transform.DOMoveX(266, 1f);
            bee2.transform.DOMoveX(263, 1.5f);

            yield return new WaitForSeconds(1f);

            yield return DialogueManager.Instance.StartDialogue(dialogue2);

            yield return LevelLoader.Instance.TransitionLevel(14);
            
            PlayerController.Instance.IsDisabled = false;
            PlayerController.Instance.InCutscene = false;
            PlayerController.Instance.StillCommitMovement = true;
        }
    }
}