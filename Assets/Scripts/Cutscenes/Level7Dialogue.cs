using System.Collections;
using Controller;
using Dialogue;
using Program.UI;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utils;

namespace Cutscenes
{
    public class Level7Dialogue: MonoBehaviour, ILevelEntranceCutscene
    {
        [SerializeField]
        private Transform beekeeper;
        [SerializeField]
        private DialogueDefinition dialogueBefore;
        [SerializeField]
        private DialogueDefinition dialogueMid;
        [SerializeField]
        private DialogueDefinition dialogueAfter;

        private Animator _bkAnim;
        private SpriteRenderer _bkSpr;
        
        [ContextMenu("START")]
        public void StartCutscene()
        {
            _bkAnim = beekeeper.gameObject.GetComponent<Animator>();
            _bkAnim.Play("Beekeeper");
            _bkSpr = beekeeper.gameObject.GetComponent<SpriteRenderer>();
            StartCoroutine(CutsceneManaged());
        }

        private IEnumerator CutsceneManaged()
        {
            PlayerController.Instance.IsDisabled = true;
            yield return new WaitForSeconds(1.5f);
            yield return DialogueManager.Instance.StartDialogue(dialogueBefore, true);
            yield return new WaitForSeconds(1.5f);
            var originalPos = beekeeper.transform.position;
            var amountToMove = 0f;
            _bkSpr.flipX = true;
            _bkAnim.Play("BeekeeperWalk");
            while (amountToMove < 3f)
            {
                amountToMove += 1.5f * Time.deltaTime;
                beekeeper.transform.position = originalPos - new Vector3(amountToMove, 0f);
                yield return null;
            }
            _bkAnim.Play("Beekeeper");
            yield return new WaitForSeconds(1f);
            yield return DialogueManager.Instance.StartDialogue(dialogueMid, true);
            yield return new WaitForSeconds(.5f);
            _bkSpr.flipX = false;
            yield return new WaitForSeconds(1f);
            yield return DialogueManager.Instance.StartDialogue(dialogueAfter, true);
            StartCoroutine(MoveBeekeeper());
            StartCoroutine(MoveTowards(new Vector3(50f, 0f, 0f)));
            yield return FadeOut();
            ProgrammableUIManager.Instance.Canvas.GetChild(6).gameObject.SetActive(true);
            yield return new WaitForSeconds(4f);
        }

        private IEnumerator FadeOut()
        {
            var black = ProgrammableUIManager.Instance.Canvas.GetChild(5);
            var img = black.gameObject.GetComponent<Image>();
            img.gameObject.SetActive(true);
            img.color = new Color(0f, 0f, 0f, 0f);
            var opacity = 0f;
            
            while (opacity < 1f)
            {
                opacity += .5f * Time.deltaTime;
                img.color = new Color(0f, 0f, 0f, opacity);
                yield return null;
            }

            img.color = Color.black;
        }


        private IEnumerator MoveBeekeeper()
        {
            var amountToMove = 0f;
            var originalPos = beekeeper.transform.position;
            _bkAnim.Play("BeekeeperWalk");
            while (amountToMove < 50f)
            {
                amountToMove += 2.5f * Time.deltaTime;
                beekeeper.transform.position = originalPos + new Vector3(amountToMove, 0f);
                yield return null;
            }
            _bkAnim.Play("Beekeeper");
        }
        
        private IEnumerator MoveTowards(Vector3 pos)
        {
            PlayerController.Instance.IsDisabled = true;
            PlayerController.Instance.StillCommitMovement = true;
            var endPos = PlayerController.Instance.transform.position + pos;
            while (Util.SqrDistance(PlayerController.Instance.transform.position, endPos) > .5f)
            {
                PlayerController.Instance.Velocity = new Vector2(7f, 0f);
                yield return null;
            }
            PlayerController.Instance.IsDisabled = false;
            PlayerController.Instance.StillCommitMovement = false;
        }
    }
}