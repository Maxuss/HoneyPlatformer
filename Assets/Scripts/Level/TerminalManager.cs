using System;
using Controller;
using TMPro;
using UnityEngine;

namespace Level
{
    public class TerminalManager: MonoBehaviour
    {
        [SerializeField]
        private Transform terminal;
        [SerializeField]
        private TMP_Text text;

        private bool _inTerminal;

        public static TerminalManager Instance { get; set; }

        private void Awake()
        {
            Instance = this;
        }

        private void Update()
        {
            if (!_inTerminal)
                return;

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                _inTerminal = false;
                // TODO: some edge case when we are in dialogue/editing?
                PlayerController.Instance.IsDisabled = false;
                terminal.gameObject.SetActive(false);
                // TODO: some close sound?
            }
        }

        public void OpenTerminal(string terminalText)
        {
            text.text = terminalText;
            _inTerminal = true;
            terminal.gameObject.SetActive(true);
        }
    }
}