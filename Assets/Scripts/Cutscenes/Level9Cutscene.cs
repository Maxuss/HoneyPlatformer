using System.Collections;
using Controller;
using Dialogue;
using UnityEngine;

namespace Cutscenes
{
    public class Level9Cutscene: MonoBehaviour
    {
        private bool _activated;

        [SerializeField]
        private DialogueDefinition dialogue;
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_activated)
                return;
            _activated = true;

            PlayerController.Instance.IsDisabled = true;
            PlayerController.Instance.StillCommitMovement = true;
            PlayerController.Instance.InCutscene = true;
            PlayerController.Instance.Velocity = Vector2.zero;

            StartCoroutine(Cutscene());
        }

        private IEnumerator Cutscene()
        {
            yield return DialogueManager.Instance.StartDialogue(dialogue);
            
            PlayerController.Instance.IsDisabled = false;
            PlayerController.Instance.StillCommitMovement = true;
            PlayerController.Instance.InCutscene = false;
        }
    }
}