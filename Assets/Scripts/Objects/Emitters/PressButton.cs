using System.Collections.Generic;
using Level;
using Program;
using Program.Channel;
using UnityEngine;
using Utils;

namespace Objects.Emitters
{
    public class PressButton: MonoBehaviour, IInteractable, IChannelSender, IActionContainer
    {
        private bool _state;
        
        [SerializeField]
        private Transform connectedReceiver;

        [SerializeField]
        private Sprite onSprite;
        [SerializeField]
        private Sprite offSprite;
        [SerializeField]
        private AudioClip toggleSound;
        
        private IChannelReceiver _rx;
        private SpriteRenderer _spriteRenderer;
        private bool _activated;

        private void OnDrawGizmosSelected()
        {
            if (connectedReceiver != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, connectedReceiver.position);
            }
        }

        public void Start()
        {
            if(connectedReceiver != null)
                _rx = connectedReceiver.GetComponent<IChannelReceiver>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _spriteRenderer.sprite = offSprite;
        }

        public void OnInteract()
        {
            if (_activated)
                return;

            _activated = true;
            _spriteRenderer.sprite = onSprite;
            SfxManager.Instance.Play(toggleSound, .4f);
            _rx?.ReceiveBool(transform, true);

            StartCoroutine(Util.Delay(() =>
            {
                _rx?.ReceiveBool(transform, false);
                _activated = false;
                _spriteRenderer.sprite = offSprite;
            }, 1));
        }

        public List<IChannelReceiver> ConnectedRx => Util.ListOf(_rx);

        [field: SerializeField]
        public bool ConnectionLocked { get; set; }
        public void Connect(IChannelReceiver rx)
        {
            _rx = rx;
            _rx.ReceiveBool(transform, _state);
        }

        public void Disconnect()
        {
            _rx = null;
        }

        public string Name => "obj.wall_btn.name";
        public string Description => "obj.wall_btn.desc";

        public ActionInfo[] SupportedActions { get; } = new[]
        {
            new ActionInfo
            {
                ActionName = "Активация",
                ActionDescription = "При активации временно выводит сигнал 1."
            }
        };

        public ProgrammableType Type { get; } = ProgrammableType.Emitter;
        public ActionData SelectedAction { get; set; }
        public void Begin(ActionData action)
        {
            // its a lever, do nothing
        }
    }
}