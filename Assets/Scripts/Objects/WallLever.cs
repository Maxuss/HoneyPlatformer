using System;
using System.Collections.Generic;
using Level;
using Program.Channel;
using UnityEngine;
using Utils;

namespace Objects
{
    public class WallLever: MonoBehaviour, IInteractable, IChannelSender
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
            _rx = connectedReceiver.GetComponent<IChannelReceiver>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _spriteRenderer.sprite = state ? onSprite : offSprite;
        }

        public void OnInteract()
        {
            state = !state;
            _spriteRenderer.sprite = state ? onSprite : offSprite;
            SfxManager.Instance.Play(toggleSound, 0.2f);
            _rx.ReceiveBool(transform, state);
        }

        public List<IChannelReceiver> ConnectedRx => Util.ListOf(_rx);
    }
}