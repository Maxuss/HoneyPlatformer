using System;
using System.Collections;
using System.Collections.Generic;
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
        private Sprite[] mefodiySprites;
        [SerializeField]
        private Sprite[] olegSprites;
        [SerializeField]
        private Sprite[] captainSprites;
        [SerializeField]
        private Sprite[] donSprites;
        [SerializeField]
        private Sprite[] krisSprites;
        [SerializeField]
        private Sprite[] sashaSprites;
        [SerializeField]
        private Sprite[] sapsanSprites;
        
        private AudioSource _as;
        
        private int _currentCharIdx;
        private int _speechIdx;
        private DialogueSpeech _speech;
        private DialogueDefinition _dialogue;
        private bool _inDialogue;
        private bool _shouldContinue;
        private bool _doNotEnableMovement;

        private Sprite[] CurrentSprites
        { 
            get 
            {
                return _dialogue.speeches[_speechIdx].speaker switch
                {
                    SpeakingPerson.Sapsan => sapsanSprites,
                    SpeakingPerson.Mefodiy => mefodiySprites,
                    SpeakingPerson.Sasha => sashaSprites,
                    SpeakingPerson.Kris => krisSprites,
                    SpeakingPerson.Oleg => olegSprites,
                    SpeakingPerson.Captain => captainSprites,
                    SpeakingPerson.Don => donSprites,
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
        }

        private void Start()
        {
            _as = GetComponent<AudioSource>();
            Instance = this;

            dialogueObject.SetActive(false);
        }
        
        public IEnumerator StartDialogue(DialogueDefinition dialogue, bool doNotEnableMovement = false)
        {
            PlayerController.Instance.IsDisabled = true;
            _dialogue = dialogue;
            _inDialogue = true;
            dialogueObject.SetActive(true);
            _doNotEnableMovement = doNotEnableMovement;
            _shouldContinue = true;
            while (_speechIdx < dialogue.speeches.Count)
            {
                _speech = dialogue.speeches[_speechIdx];
                StartCoroutine(SingleSpeech(dialogue.speeches[_speechIdx]));
                var shouldExitOnPress = _speechIdx + 1 >= dialogue.speeches.Count;
                while (_shouldContinue && _inDialogue)
                {
                    if (Input.GetKeyDown(KeyCode.Return))
                    {
                        if (_currentCharIdx < _speech.text.Length)
                        {
                            _currentCharIdx = _speech.text.Length;
                            text.text = _speech.text;
                            _as.Stop();
                        }
                        else
                        {
                            if (shouldExitOnPress)
                            {
                                _inDialogue = false;
                            }

                            _shouldContinue = false;
                        }
                        speakerSprite.sprite = CurrentSprites[0];
                    }
                    yield return null;
                }
                
                if (!_inDialogue)
                {
                    text.text = "";
                    _currentCharIdx = 0;
                    _dialogue = null;
                    _speech = null;
                    _inDialogue = false;
                    dialogueObject.SetActive(false);
                    PlayerController.Instance.IsDisabled = _doNotEnableMovement;
                    _speechIdx = 0;
                    yield break;
                }
                
                _speechIdx++;
                _shouldContinue = true;
            }
        }

        // FIXED BUG HERE
        private Coroutine _animationCoroutine;
        private IEnumerator SingleSpeech(DialogueSpeech dialogue)
        {
            if(_animationCoroutine != null)
                StopCoroutine(_animationCoroutine);
            _animationCoroutine = StartCoroutine(StartAnimation(CurrentSprites));
            characterName.text = dialogue.characterName;
            text.text = "";
            _currentCharIdx = 0;
            _as.clip = dialogue.audio;
            _as.time = 0f;
            _as.Play();
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
                _currentCharIdx += 1;
                yield return new WaitForSeconds(0.06f * dialogue.speedModifier);
            }

            yield return new WaitUntil(() => !_as.isPlaying);
            StopCoroutine(_animationCoroutine);
            _animationCoroutine = null;
            speakerSprite.sprite = CurrentSprites[0];
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