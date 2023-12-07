using System;
using Program.Channel;
using UnityEngine;

namespace Objects
{
    public class BinaryCombinator: MonoBehaviour, IBiChannelReceiver
    {
        // TODO: program the operation
        [SerializeField]
        private BinaryOperation operation;
        
        [SerializeField]
        private Transform sourceLeft;
        [SerializeField]
        private Transform sourceRight;

        [SerializeField]
        private Transform connectedReceiver;

        Transform IBiChannelReceiver.SourceLeft => sourceLeft;

        Transform IBiChannelReceiver.SourceRight => sourceRight;

        private bool _left;
        private bool _right;
        private bool _state;
        private IChannelReceiver _rx;
        private Renderer _renderer;
        private static readonly int Output = Shader.PropertyToID("_Output");
        private static readonly int Left = Shader.PropertyToID("_Left");
        private static readonly int Right = Shader.PropertyToID("_Right");

        private void Start()
        {
            _rx = connectedReceiver.GetComponent<IChannelReceiver>();
            _renderer = GetComponent<Renderer>();
        }

        public void ReceiveBool(MessageDirection direction, Transform src, bool b)
        {
            if (direction == MessageDirection.Left)
            {
                _left = b;
                _renderer.material.SetFloat(Left, b ? 1f : 0f);
            }
            else
            {
                _right = b;
                _renderer.material.SetFloat(Right, b ? 1f : 0f);
            }

            RecalculateOutput();
        }

        public void ReceiveFloat(MessageDirection direction, Transform src, float v)
        {
            // do not support receiving float values
        }

        private void RecalculateOutput()
        {
            _state = operation switch
            {
                BinaryOperation.Or => _left || _right,
                BinaryOperation.And => _left && _right,
                BinaryOperation.Xor => _left ^ _right,
                _ => _state
            };
            
            _rx.ReceiveBool(transform, _state);
            _renderer.material.SetFloat(Output, _state ? 1f : 0f);
        }
    }

    public enum BinaryOperation
    {
        Or,
        And,
        Xor
    }
}