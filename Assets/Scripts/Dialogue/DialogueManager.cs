using System;
using System.Collections;
using System.Linq;
using Controller;
using Level;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Utils;
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
        private Image speakerSprite;
        [SerializeField]
        private TMP_Text characterName;

        [SerializeField]
        private DialogueDefinition testDialogue;
        
        private int _currentCharIdx;
        private int _speechIdx;
        private DialogueSpeech _speech;
        private DialogueDefinition _dialogue;
        private bool _inDialogue;
        private bool _shouldContinue;

        private void Start()
        {
            Instance = this;
            dialogueObject.SetActive(false);
            
            StartCoroutine(Util.Delay(() => StartCoroutine(StartDialogue(testDialogue)), 2f));
        }
        
        private void Update()
        {
            if (!_inDialogue)
                return;

            if (!Input.GetKeyDown(KeyCode.Return)) return;
            
            if (_currentCharIdx < _speech.text.Length)
            {
                _currentCharIdx = _speech.text.Length;
                text.text = _speech.text;
            }
            else
            {
                if (_speechIdx + 1 >= _dialogue.Speeches.Count)
                {
                    text.text = "";
                    _currentCharIdx = 0;
                    _dialogue = null;
                    _speech = null;
                    _speechIdx = 0;
                    _inDialogue = false;
                    dialogueObject.SetActive(false);
                    PlayerController.Instance.IsDisabled = false;
                }
                else
                {
                    _shouldContinue = true;
                }
            }
        }
        
        public IEnumerator StartDialogue(DialogueDefinition dialogue)
        {
            PlayerController.Instance.IsDisabled = true;
            _dialogue = dialogue;
            _inDialogue = true;
            dialogueObject.SetActive(true);
            for (_speechIdx = 0; _speechIdx < dialogue.Speeches.Count; _speechIdx++)
            {
                _speech = dialogue.Speeches[_speechIdx];
                yield return StartCoroutine(SingleSpeech(dialogue.Speeches[_speechIdx]));
                while (!_shouldContinue)
                {
                    yield return null;
                }

                _shouldContinue = false;
            }
        }

        private IEnumerator SingleSpeech(DialogueSpeech dialogue)
        {
            var animationCoroutine = StartCoroutine(StartAnimation(dialogue.sprites));
            characterName.text = dialogue.characterName;
            text.text = "";
            _currentCharIdx = 0;
            while (_currentCharIdx < dialogue.text.Length && _inDialogue)
            {
                var ch = dialogue.text[_currentCharIdx];
                while (ch == ' ')
                {
                    _currentCharIdx += 1;
                    if (_currentCharIdx > dialogue.text.Length)
                    {
                        text.text = dialogue.text;
                    }
                    ch = dialogue.text[_currentCharIdx];
                }
                text.text = dialogue.text.Substring(0, _currentCharIdx + 1);
                var sound = dialogue.audioSounds[Random.Range(0, dialogue.audioSounds.Length)];
                SfxManager.Instance.Play(sound);
                _currentCharIdx += 1;
                yield return new WaitForSeconds(0.06f * dialogue.speedModifier);
            }
            StopCoroutine(animationCoroutine);
            speakerSprite.sprite = dialogue.sprites[0];
        }

        private IEnumerator StartAnimation(Sprite[] frames)
        {
            var frame = 0;
            while (true)
            {
                frame = (frame + 1) % frames.Length;
                speakerSprite.sprite = frames[frame];
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}