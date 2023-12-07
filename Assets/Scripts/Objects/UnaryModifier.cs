using System;
using Program.Channel;
using UnityEngine;

namespace Objects
{
    public class UnaryModifier: MonoBehaviour, IChannelReceiver
    {
        [SerializeField]
        private UnaryOperator op;

        [SerializeField]
        private Transform connectedReceiver;
        
        private IChannelReceiver _rx;
        private Renderer _renderer;

        private bool _state;
        private static readonly int Output = Shader.PropertyToID("_Output");
        private static readonly int InputData = Shader.PropertyToID("_Input");

        private void Start()
        {
            _rx = connectedReceiver.GetComponent<IChannelReceiver>();
            _renderer = GetComponent<Renderer>();
        }

        public void ReceiveBool(Transform source, bool b)
        {
            _state = op == UnaryOperator.Identity ? b : !b;
            _renderer.material.SetFloat(InputData, b ? 1f : 0f);
            _renderer.material.SetFloat(Output, _state ? 1f : 0f);
            _rx.ReceiveBool(transform, _state);
        }

        public void ReceiveFloat(Transform source, float v)
        {
            // We don't support floats yet
        }
    }

    public enum UnaryOperator
    {
        Identity,
        Not,
    }
}