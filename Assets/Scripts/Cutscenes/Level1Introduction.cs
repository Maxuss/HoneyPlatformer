using System.Collections;
using Controller;
using Dialogue;
using Level;
using Objects;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using Utils;

namespace Cutscenes
{
    public class Level1Introduction: MonoBehaviour
    {
        [SerializeField]
        private AudioClip alarmClip;
        [SerializeField]
        private DialogueDefinition dialogue;
        [SerializeField]
        private AudioClip toggleSound;
        [SerializeField]
        private AlarmLight[] lights;
        [SerializeField]
        private Transform[] lightEnable;

        public void Start()
        {
            PlayerController.Instance.IsDisabled = true;
            PlayerController.Instance.StillCommitMovement = true;
            PlayerController.Instance.InCutscene = true;
            PlayerController.Instance.BlackOut();
            
            StartCoroutine(MusicManager.Instance.Crossfade(alarmClip, .3f, .05f));
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
            PlayerController.Instance.IsDisabled = true;
            PlayerController.Instance.StillCommitMovement = true;
        }

        private IEnumerator CombinedCutscene()
        {
            yield return PlayerController.Instance.FadeOut();
            yield return Cutscene();
        }
        
        private IEnumerator Cutscene()
        {
            yield return new WaitForSeconds(1f);
            yield return MoveTowards(new Vector3(5f, 0f));
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
            yield return DialogueManager.Instance.StartDialogue(dialogue, true);
            StartCoroutine(Util.Delay(() => MusicManager.Instance.NextAmbientTrack(), 2f));
            StartCoroutine(Util.Delay(() => ToastManager.Instance.ShowToast("E - взаимодействовать с объектом"), 3f));

            PlayerController.Instance.IsDisabled = false;
            PlayerController.Instance.StillCommitMovement = false;
            PlayerController.Instance.InCutscene = false;
        }
    }
}