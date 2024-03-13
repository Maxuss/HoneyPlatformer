using System.Collections;
using Controller;
using Dialogue;
using Level;
using Objects;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using Utils;

namespace Cutscenes
{
    public class Level1Introduction: MonoBehaviour
    {
        [SerializeField]
        private AudioClip alarmClip;
        [SerializeField]
        private Animator playerAnimator;
        [SerializeField]
        private PixelPerfectCamera ppc;
        [SerializeField]
        private DialogueDefinition dialogue1;
        [SerializeField]
        private DialogueDefinition dialogue2;
        [SerializeField]
        private AudioClip toggleSound;
        [SerializeField]
        private AlarmLight[] lights;
        [SerializeField]
        private Transform[] lightEnable;
        [SerializeField]
        private Image blackCanvas;
        [SerializeField]
        private SpriteRenderer chair;
        [SerializeField]
        private bool disabled;

        public void Start()
        {
            if (disabled)
            {
                chair.color = Color.white;
                blackCanvas.gameObject.SetActive(false);
                StartCoroutine(Util.Delay(() => MusicManager.Instance.NextAmbientTrack(), 2f));
                StartCoroutine(Util.Delay(() => ToastManager.Instance.ShowToast("E - взаимодействовать с объектом"), 3f));
                return;
            }
            chair.color = new Color(1f, 1f, 1f, 0f);
            PlayerController.Instance.IsDisabled = true;
            StartCoroutine(Util.Delay(() => StartCoroutine(CombinedCutscene()), 2f));
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

        private IEnumerator CombinedCutscene()
        {
            StartCoroutine(MusicManager.Instance.Crossfade(alarmClip, .3f, .05f));
            playerAnimator.Play("Sleep");
            yield return FadeIn();
            yield return Cutscene();
        }

        private IEnumerator FadeIn()
        {
            var opacity = 1f;
            
            while (opacity > 0f)
            {
                opacity -= .4f * Time.deltaTime;
                blackCanvas.color = new Color(0f, 0f, 0f, opacity);
                yield return null;
            }
            blackCanvas.gameObject.SetActive(false);
        }

        private IEnumerator Cutscene()
        {
            playerAnimator.Play("PlayerIdle");
            chair.color = new Color(1f, 1f, 1f, 1f);
            yield return new WaitForSeconds(1f);
            yield return DialogueManager.Instance.StartDialogue(dialogue1, true);
            yield return MoveTowards(new Vector3(5f, 0f));
            PlayerController.Instance.IsDisabled = true;
            yield return new WaitForSeconds(.5f);
            SfxManager.Instance.Play(toggleSound, .5f); 
            MusicManager.Instance.StopAbruptly();
            foreach (var light2d in lights)
            {
                light2d.transform.GetChild(0).gameObject.SetActive(false);
                light2d.transform.GetChild(1).gameObject.SetActive(false);
            }

            foreach (var light2d in lightEnable)
            {
                light2d.transform.GetChild(0).gameObject.SetActive(true);
            }
            yield return new WaitForSeconds(1f);
            yield return DialogueManager.Instance.StartDialogue(dialogue2, true);
            StartCoroutine(Util.Delay(() => MusicManager.Instance.NextAmbientTrack(), 2f));
            StartCoroutine(Util.Delay(() => ToastManager.Instance.ShowToast("E - взаимодействовать с объектом"), 3f));

            PlayerController.Instance.IsDisabled = false;
            PlayerController.Instance.StillCommitMovement = false;
        }
    }
}