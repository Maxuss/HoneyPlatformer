using System;
using System.Collections.Generic;
using Program;
using Program.Channel;
using UnityEngine;
using Utils;

namespace Objects.Processors
{
    public class SignalSplitter: MonoBehaviour, IChannelReceiver, IActionContainer, IChannelSender
    {
        [SerializeField]
        private Transform receiver1;
        [SerializeField]
        private Transform receiver2;
        private bool _state;
        private IChannelReceiver _rx1;
        private IChannelReceiver _rx2;

        private Renderer _renderer;
        
        private static readonly int Input = Shader.PropertyToID("_Input");
        
        public void ReceiveBool(Transform source, bool b)
        {
            _state = b;
            _rx1.ReceiveBool(transform, _state);
            StartCoroutine(Util.DelayFrames(() => _rx2.ReceiveBool(transform, _state), 3));
            _renderer.material.SetFloat(Input, b ? 1f : 0f);
        }

        private void OnDrawGizmosSelected()
        {
            if (receiver1 != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, receiver1.position);
            }

            if (receiver2 != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, receiver2.position);
            }
        }

        private void Start()
        {
            _rx1 = receiver1.GetComponent<IChannelReceiver>();
            _rx2 = receiver2.GetComponent<IChannelReceiver>();
            _renderer = GetComponent<Renderer>();
        }

        public void ReceiveFloat(Transform source, float v)
        {
        }

        public string Name => "Разделитель сигнала";
        public string Description => "Разделяет сигнал со входа и передает его двум подключенным объектам";

        public ActionInfo[] SupportedActions { get; } = new[]
        {
            new ActionInfo
            {
                ActionName = "Разделить",
                ActionDescription = "Делит сигнал на два вывода"
            }
        };

        public ProgrammableType Type => ProgrammableType.Processor;
        public ActionData SelectedAction { get; set; }
        public void Begin(ActionData action)
        {
            // nothing to do
        }

        public List<IChannelReceiver> ConnectedRx => Util.ListOf(_rx1, _rx2);
        public bool ConnectionLocked { get; set; } = true;
        public void Connect(IChannelReceiver rx)
        {
            // TODO: connection?
        }

        public void Disconnect()
        {
        }
    }
}