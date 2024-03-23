using System;
using System.Collections.Generic;
using Program;
using Program.Channel;
using UnityEngine;
using UnityEngine.Serialization;
using Utils;

namespace Objects.Processors
{
    public class BinaryCombinator: MonoBehaviour, IBiChannelReceiver, IActionContainer, IChannelSender
    {
        // TODO: program the operation
        [SerializeField]
        private BinaryOperation operation;
        
        [SerializeField]
        private Transform connectedReceiver;
        
        
        [field: SerializeField]
        [field: FormerlySerializedAs("sourceLeft")]
        public Transform SourceLeft { get; set; }
        [field: SerializeField]
        [field: FormerlySerializedAs("sourceRight")]
        public Transform SourceRight { get; set; }

        private bool _swapRight;
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
            if(connectedReceiver != null)
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
                BinaryOperation.Implication => !_left || _right,
                BinaryOperation.Equality => _left == _right,
                BinaryOperation.Nand => !(_left && _right),
                BinaryOperation.Nor => !(_left || _right),
                _ => _state
            };
            
            _rx?.ReceiveBool(transform, _state);
            _renderer.material.SetFloat(Output, _state ? 1f : 0f);
        }

        public string Name => "Логический Комбинатор";
        public string Description => "Применяет логическую операцию к двум сигналам на входе и выводит результат.";

        public ActionInfo[] SupportedActions { get; } = new[]
        {
            new ActionInfo
            {
                ActionName = "OR",
                ActionDescription = "Применяет логическую операцию ИЛИ"
            },
            new ActionInfo
            {
                ActionName = "AND",
                ActionDescription = "Применяет логическую операцию И"
            },
            new ActionInfo
            {
                ActionName = "XOR",
                ActionDescription = "Применяет логическую операцию Исключающего ИЛИ"
            },
            new ActionInfo
            {
                ActionName = "Импликация",
                ActionDescription = "Применяет логическую операцию импликации."
            },
            new ActionInfo
            {
                ActionName = "Эквиваленция",
                ActionDescription = "Применяет логическую операцию эквиваленции (приравнивания)."
            },
            new ActionInfo
            {
                ActionName = "NAND",
                ActionDescription = "Применяет логическую операцию отрицания И"
            },
            new ActionInfo
            {
                ActionName = "NOR",
                ActionDescription = "Применяет логическую операцию отрицания ИЛИ"
            }
        };

        public ProgrammableType Type { get; } = ProgrammableType.Processor;
        [field: SerializeField]
        public ActionData SelectedAction { get; set; }
        public void Begin(ActionData action)
        {
            var newOp = (BinaryOperation) Enum.ToObject(typeof(BinaryOperation), action.ActionIndex);
            if (newOp == operation)
                return;

            operation = newOp;
            RecalculateOutput();
        }

        public List<IChannelReceiver> ConnectedRx => Util.ListOf(_rx);
        [field: SerializeField]
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

    public enum BinaryOperation
    {
        Or,
        And,
        Xor,
        Implication,
        Equality,
        Nand,
        Nor
    }
}