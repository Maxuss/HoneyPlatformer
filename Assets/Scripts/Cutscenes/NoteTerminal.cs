using System;
using Controller;
using Level;
using Objects;
using TMPro;
using UnityEngine;

namespace Cutscenes
{
    public class NoteTerminal: MonoBehaviour, IInteractable
    {
        [SerializeField]
        [TextArea]
        private string text;

        private Transform _indicator;
        private bool _interacted;
        
        private float _indicatorY;

        private void Start()
        {
            _indicator = transform.GetChild(0);
        }

        private void Update()
        {
            if (_interacted)
                return;
            _indicatorY += Time.deltaTime;
            var y = MathF.Sin(_indicatorY * 3f) * 0.2f;
            _indicator.localPosition = new Vector3(0f, 0.5f + y, 0f);
            _indicator.rotation = Quaternion.Euler(new Vector3(0f, _indicatorY * 60f, 0f));
        }

        public void OnInteract()
        {
            _interacted = true;
            _indicator.gameObject.SetActive(false);
            PlayerController.Instance.IsDisabled = true;
            TerminalManager.Instance.OpenTerminal(text);
        }
    }
}