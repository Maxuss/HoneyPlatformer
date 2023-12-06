using System;
using Program;
using Program.Action;
using Program.Channel;
using Program.Trigger;
using UnityEngine;
using Utils;

namespace Objects
{
    public class FloorButton: MonoBehaviour, IParentCollisionHandler
    {
        [SerializeField]
        private FloorButtonCollisionDetection detection;

        [SerializeField]
        private Transform connectedReceiver;

        [SerializeField]
        private Sprite pressedSprite;
        [SerializeField]
        private Sprite unpressedSprite;

        private IChannelReceiver _rx;
        private SpriteRenderer _spriteRenderer;
        private bool _isPressed;
        private Collider2D _pressed;

        private void Start()
        {
            // TODO: some debug messages here? (like when channel rx is not connected)
            _rx = connectedReceiver.GetComponent<IChannelReceiver>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }
        
        public void OnChildTriggerEnter(Collider2D other)
        {
            if (_isPressed)
                return;

            _pressed = other;

            if (
                detection == FloorButtonCollisionDetection.Anything ||
                (detection == FloorButtonCollisionDetection.Box && other.CompareTag("Box")) ||
                (detection == FloorButtonCollisionDetection.Player && other.CompareTag("Player"))
                )
            {
                _rx.ReceiveBool(true);
                _isPressed = true;
                _spriteRenderer.sprite = pressedSprite;
            }
        }

        public void OnChildTriggerExit(Collider2D other)
        {
            if (!_isPressed || _pressed != other) return;
            
            _isPressed = false;
            _rx.ReceiveBool(false);
            _spriteRenderer.sprite = unpressedSprite;
        }

        public enum FloorButtonCollisionDetection
        {
            Anything,
            Player,
            Box,
        }
    }
}