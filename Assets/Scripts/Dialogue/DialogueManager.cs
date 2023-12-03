using System;
using System.Collections;
using Controller;
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

        private int _currentCharIdx;
        private DialogueDefinition _dialogue;
        private bool _inDialogue;

        private void Start()
        {
            Instance = this;
            dialogueObject.SetActive(false);
        }
        
        private void Update()
        {
            if (!_inDialogue)
                return;
            
            if (Input.GetKeyDown(KeyCode.Return))
            {
                if (_currentCharIdx < _dialogue.text.Length)
                {
                    _currentCharIdx = _dialogue.text.Length;
                    text.text = _dialogue.text;
                }
                else
                {
                    text.text = "";
                    _currentCharIdx = 0;
                    _dialogue = null;
                    _inDialogue = false;
                    dialogueObject.SetActive(false);
                    PlayerController.Instance.IsDisabled = false;
                }
            }
        }
        
        public IEnumerator StartDialogue(DialogueDefinition dialogue)
        {
            PlayerController.Instance.IsDisabled = true;
            _dialogue = dialogue;
            _inDialogue = true;
            dialogueObject.SetActive(true);
            while (_currentCharIdx < dialogue.text.Length && _inDialogue)
            {
                var ch = dialogue.text[_currentCharIdx];
                while (ch == ' ')
                {
                    _currentCharIdx += 1;
                    if (_currentCharIdx > _dialogue.text.Length)
                    {
                        text.text = dialogue.text;
                        yield return null;
                    }
                    ch = dialogue.text[_currentCharIdx];
                }
                text.text = dialogue.text.Substring(0, _currentCharIdx + 1);
                var sound = dialogue.audioSounds[Random.Range(0, dialogue.audioSounds.Count)];
                audioSource.PlayOneShot(sound);
                _currentCharIdx += 1;
                yield return new WaitForSeconds(0.06f);
            }
        }
    }
}