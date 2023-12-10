using System.Collections.Generic;
using Level;
using Program;
using Program.Channel;
using UnityEngine;
using Utils;

namespace Objects
{
    public class WallLever: MonoBehaviour, IInteractable, IChannelSender, IActionContainer
    {
        [SerializeField]
        private bool state;
        
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

        public void Start()
        {
            if(connectedReceiver != null)
                _rx = connectedReceiver.GetComponent<IChannelReceiver>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _spriteRenderer.sprite = state ? onSprite : offSprite;
        }

        public void OnInteract()
        {
            state = !state;
            _spriteRenderer.sprite = state ? onSprite : offSprite;
            SfxManager.Instance.Play(toggleSound, 0.2f);
            _rx?.ReceiveBool(transform, state);
        }

        public List<IChannelReceiver> ConnectedRx => Util.ListOf(_rx);

        [field: SerializeField]
        public bool ConnectionLocked { get; set; }
        public void Connect(IChannelReceiver rx)
        {
            _rx = rx;
            _rx.ReceiveBool(transform, state);
        }

        public void Disconnect()
        {
            _rx = null;
        }

        public string Name => "Рычаг";
        public string Description => "При нажатии выводит сигнал 1/0 в зависимости от нового состояния.";

        public ActionInfo[] SupportedActions { get; } = new[]
        {
            new ActionInfo
            {
                ActionName = "Активация",
                ActionDescription = "При активации выводит сигнал 1, при деактивации - 0."
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