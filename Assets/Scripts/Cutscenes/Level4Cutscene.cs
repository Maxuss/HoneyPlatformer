using System.Collections;
using Controller;
using Dialogue;
using Level;
using NPC;
using UnityEngine;
using Utils;

namespace Cutscenes
{
    public class Level4Cutscene: MonoBehaviour, ILevelEntranceCutscene
    {
        [SerializeField] private AudioClip alarmClip;
        [SerializeField] private NpcController beekeeper;

        [SerializeField]
        private DialogueDefinition dialogue;
        
        public void StartCutscene()
        {
            PlayerController.Instance.Velocity = new Vector2();
            PlayerController.Instance.IsDisabled = true;
            PlayerController.Instance.StillCommitMovement = true;
            PlayerController.Instance.InCutscene = true;
            StartCoroutine(Util.Delay(() => StartCoroutine(Cutscene()), 2f));
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
            yield return MoveTowards(new Vector3(5f, 0f));
            yield return beekeeper.Walk(5, .3f);
            yield return new WaitForSeconds(2);
            yield return beekeeper.Walk(-5, .3f);
            yield return new WaitForSeconds(2);
            // yield return DialogueManager.Instance.StartDialogue(dialogue, true);
            // MusicManager.Instance.StopAbruptly();
            // yield return MusicManager.Instance.Crossfade(alarmClip, .2f, .05f);
            // PlayerController.Instance.BlackOut();
            // yield return new WaitForSeconds(1f);
            // SfxManager.Instance.Play(explosion);
            // PlayerController.Instance.ShowTBC();
        }
    }
}