using System;
using System.Collections.Generic;
using Level;
using Program;
using Program.Channel;
using UnityEngine;
using Utils;

namespace Objects.Emitters
{
    public class FloorButton: MonoBehaviour, IParentCollisionHandler, IActionContainer, IChannelSender
    {
        [SerializeField]
        private FloorButtonCollisionDetection detection;

        [SerializeField]
        private Transform connectedReceiver;

        [SerializeField]
        private Sprite pressedSprite;
        [SerializeField]
        private Sprite unpressedSprite;

        [SerializeField]
        private AudioClip pressedSound;
        [SerializeField]
        private AudioClip unpressedSound;

        private IChannelReceiver _rx;
        private SpriteRenderer _spriteRenderer;
        private bool _isPressed;
        private Collider2D _pressed;

        private void OnDrawGizmosSelected()
        {
            if (connectedReceiver != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, connectedReceiver.position);
            }
        }

        private void Start()
        {
            // TODO: some debug messages here? (like when channel rx is not connected)
            if(connectedReceiver != null)
                _rx = connectedReceiver.GetComponent<IChannelReceiver>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }
        
        public void OnChildTriggerEnter(Collider2D other)
        {
            if (_isPressed)
                return;
            
            if (
                detection == FloorButtonCollisionDetection.Anything ||
                (detection == FloorButtonCollisionDetection.Box && other.CompareTag("Box")) ||
                (detection == FloorButtonCollisionDetection.Player && other.CompareTag("Player"))
                )
            {
                _pressed = other;
                _rx?.ReceiveBool(transform, true);
                _isPressed = true;
                _spriteRenderer.sprite = pressedSprite;
                SfxManager.Instance.Play(pressedSound, 0.2f);
            }
        }

        public void OnChildTriggerExit(Collider2D other)
        {
            if (!_isPressed || _pressed != other) return;
            
            _isPressed = false;
            _rx?.ReceiveBool(transform, false);
            _spriteRenderer.sprite = unpressedSprite;
            SfxManager.Instance.Play(unpressedSound, 0.2f);
        }

        public void OnChildTriggerStay(Collider2D other)
        {
            if (_isPressed)
                return;
            
            if (
                detection == FloorButtonCollisionDetection.Anything ||
                (detection == FloorButtonCollisionDetection.Box && other.CompareTag("Box")) ||
                (detection == FloorButtonCollisionDetection.Player && other.CompareTag("Player"))
            )
            {
                _pressed = other;
                _rx?.ReceiveBool(transform, true);
                _isPressed = true;
                _spriteRenderer.sprite = pressedSprite;
                SfxManager.Instance.Play(pressedSound, 0.2f);
            }
        }

        public List<IChannelReceiver> ConnectedRx => Util.ListOf(_rx);
        [field: SerializeField]
        public bool ConnectionLocked { get; set; }
        public void Connect(IChannelReceiver rx)
        {
            _rx = rx;
            _rx.ReceiveBool(transform, _isPressed);
        }

        public void Disconnect()
        {
            _rx = null;
        }

        public string Name => "Нажимная кнопка";

        public string Description =>
            "Активируется при нажатии игроком или определенным предметом в зависимости от настройки.";

        public ActionInfo[] SupportedActions { get; } = new[]
        {
            new ActionInfo
            {
                ActionName = "Любая активация",
                ActionDescription = "Активируется когда любой объект находится на кнопке"
            },
            new ActionInfo
            {
                ActionName = "Активация игроком",
                ActionDescription = "Активируется когда игрок наступает на кнопку",
            },
            new ActionInfo
            {
                ActionName = "Активация тяж. кубом",
                ActionDescription = "Активируется когда на кнопке находится утяжеленный куб",
            },
        };

        public ProgrammableType Type { get; } = ProgrammableType.Emitter;
        [field: SerializeField]
        public ActionData SelectedAction { get; set; }
        public void Begin(ActionData action)
        {
            detection = (FloorButtonCollisionDetection) action.ActionIndex;
        }
    }
    
    public enum FloorButtonCollisionDetection
    {
        Anything,
        Player,
        Box,
    }
}