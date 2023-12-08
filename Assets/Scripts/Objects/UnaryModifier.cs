using System;
using System.Collections.Generic;
using Program;
using Program.Channel;
using UnityEngine;
using Utils;

namespace Objects
{
    public class UnaryModifier: MonoBehaviour, IChannelReceiver, IActionContainer, IChannelSender
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
            _rx?.ReceiveBool(transform, _state);
        }
        
        public void ReceiveFloat(Transform source, float v)
        {
            // We don't support floats yet
        }

        public string Name => "Модификатор сигнала";
        public string Description => "Модификатор применяет определенную операцию к получаемому сигналу и выводит его.";

        public ActionInfo[] SupportedActions => new[]
        {
            new ActionInfo
            {
                ActionName = "Идентичность",
                ActionDescription = "Выводит тот же сигнал, что и был получен на входе."
            },
            new ActionInfo
            {
                ActionName = "Отрицание",
                ActionDescription = "Выводит сигнал, отрицательный полученному на входе."
            }
        };

        public ProgrammableType Type => ProgrammableType.Emitter;
        public ActionData SelectedAction { get; set; }
        public void Begin(ActionData action)
        {
            var newOperator = (UnaryOperator) Enum.ToObject(typeof(UnaryOperator), action.ActionIndex);
            if (op == newOperator)
            {
                return;
            }

            op = newOperator;
            _state = !_state;
            _renderer.material.SetFloat(Output, _state ? 1f : 0f);
            _rx?.ReceiveBool(transform, _state);
        }

        public List<IChannelReceiver> ConnectedRx => Util.ListOf(_rx);
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
    }

    public enum UnaryOperator
    {
        Identity,
        Not,
    }
}