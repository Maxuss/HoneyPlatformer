using System;
using System.Collections.Generic;
using Level;
using Program;
using Program.Channel;
using UnityEngine;
using Utils;

namespace Objects
{
    public class FloorButton: MonoBehaviour, IParentCollisionHandler, IChannelSender
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
            
            if (
                detection == FloorButtonCollisionDetection.Anything ||
                (detection == FloorButtonCollisionDetection.Box && other.CompareTag("Box")) ||
                (detection == FloorButtonCollisionDetection.Player && other.CompareTag("Player"))
                )
            {
                _pressed = other;
                _rx.ReceiveBool(transform, true);
                _isPressed = true;
                _spriteRenderer.sprite = pressedSprite;
                SfxManager.Instance.Play(pressedSound, 0.2f);
            }
        }

        public void OnChildTriggerExit(Collider2D other)
        {
            if (!_isPressed || _pressed != other) return;
            
            _isPressed = false;
            _rx.ReceiveBool(transform, false);
            _spriteRenderer.sprite = unpressedSprite;
            SfxManager.Instance.Play(unpressedSound, 0.2f);
        }

        public enum FloorButtonCollisionDetection
        {
            Anything,
            Player,
            Box,
        }

        public List<IChannelReceiver> ConnectedRx => Util.ListOf(_rx);
    }
}