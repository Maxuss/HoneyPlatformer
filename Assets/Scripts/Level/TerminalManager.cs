using System;
using Controller;
using TMPro;
using UnityEngine;
using Utils;

namespace Level
{
    public class TerminalManager: MonoBehaviour
    {
        [SerializeField]
        private Transform terminal;
        [SerializeField]
        private TMP_Text text;

        public bool InTerminal { get; private set; }

        public static TerminalManager Instance { get; set; }

        private void Awake()
        {
            Instance = this;
        }

        private void Update()
        {
            if (!InTerminal)
                return;

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                // TODO: some edge case when we are in dialogue/editing?
                terminal.gameObject.SetActive(false);
                // TODO: some close sound?
                PlayerController.Instance.IsDisabled = false;
                // dont even ask,,.
                StartCoroutine(Util.DelayFrames(() => InTerminal = false, 1));
            }
        }

        public void OpenTerminal(string terminalText)
        {
            text.text = terminalText;
            InTerminal = true;
            terminal.gameObject.SetActive(true);
        }
    }
}