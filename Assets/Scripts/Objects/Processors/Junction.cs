using System;
using System.Collections.Generic;
using Program;
using Program.Channel;
using UnityEngine;
using Utils;

namespace Objects.Processors
{
    public class Junction: MonoBehaviour, IChannelReceiver, IActionContainer, IChannelSender
    {
        [SerializeField]
        private Transform connectedReceiver;
        
        private IChannelReceiver _rx;
        private Renderer _renderer;
        
        private bool _state;
        private static readonly int InputData = Shader.PropertyToID("_Input");

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
            if(connectedReceiver != null)
                _rx = connectedReceiver.GetComponent<IChannelReceiver>();
            _renderer = GetComponent<Renderer>();
        }
        
        public void ReceiveBool(Transform source, bool b)
        {
            _state = b;
            _renderer.material.SetFloat(InputData, b ? 1f : 0f);
            _rx.ReceiveBool(transform, _state);
        }

        public void ReceiveFloat(Transform source, float v)
        {
            throw new System.NotImplementedException();
        }

        public string Name => "Распределитель";
        public string Description => "Проводит сигнал сквозь себя без преобразований";

        public ActionInfo[] SupportedActions { get; } = {
            new ActionInfo
            {
                ActionName = "Идентичность",
                ActionDescription = "Выводит тот же сигнал, что и был получен на входе."
            }
        };
        public ProgrammableType Type => ProgrammableType.Processor;
        [field: SerializeField]
        public ActionData SelectedAction { get; set; }
        public void Begin(ActionData action)
        {
            // noop
        }

        public List<IChannelReceiver> ConnectedRx => Util.ListOf(_rx);
        public bool ConnectionLocked { get; set; } = true;
        public void Connect(IChannelReceiver rx)
        {
            _rx = rx;
            _rx.ReceiveBool(transform, _state);
        }

        public void Disconnect()
        {
            _rx = null;
        }
    }
}