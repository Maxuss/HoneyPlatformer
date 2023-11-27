using System;
using System.Collections;
using TMPro;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Dialogue
{
    public class DialogueManager: MonoBehaviour
    {
        public static DialogueManager Instance { get; set; }

        [SerializeField]
        private GameObject dialogueObject;
        [SerializeField]
        private TMP_Text text;
        [SerializeField]
        private AudioSource audioSource;

        [SerializeField]
        private DialogueDefinition testDialogue;

        private int _currentCharIdx = 0;
        private DialogueDefinition _dialogue;

        private void Start()
        {
            Instance = this;
            dialogueObject.SetActive(false);
        }

        [ContextMenu("TestDialogue")]
        void TestDialogue()
        {
            StartCoroutine(StartDialogue(testDialogue));
        }
        
        public IEnumerator StartDialogue(DialogueDefinition dialogue)
        {
            _dialogue = dialogue;
            dialogueObject.SetActive(true);
            while (_currentCharIdx < dialogue.text.Length)
            {
                _currentCharIdx += 1;
                text.text = dialogue.text.Substring(0, _currentCharIdx);
                var sound = dialogue.audioSounds[Random.Range(0, dialogue.audioSounds.Count)];
                audioSource.PlayOneShot(sound);
                yield return new WaitForSeconds(0.1f);
            }

            text.text = "";
            _currentCharIdx = 0;
            _dialogue = null;
            dialogueObject.SetActive(false);
        }
    }
}