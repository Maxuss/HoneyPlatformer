using System;
using Program;
using Program.Channel;
using UnityEngine;
using Utils;

namespace Objects
{
    public class ConveyorGroup: MonoBehaviour, IChannelReceiver, IActionContainer
    {
        public ConveyorBelt[] InnerBelts { get; private set; }

        private bool _enabled;
        private float _speed = 1f;
        private ConveyorBeltBehaviour _behaviour;

        private void Start()
        {
            InnerBelts = GetComponentsInChildren<ConveyorBelt>();
        }

        public void ReceiveBool(Transform source, bool b)
        {
            _enabled = b;
            foreach (var child in InnerBelts)
            {
                child.Toggle(_enabled);
            }
        }

        public void ReceiveFloat(Transform source, float v)
        {
            // doing nothing with the floats
        }

        public string Name => "Конвейер";
        public string Description => "Постоянно перемещает объекты и сущностей на нем согласно заданным параметрам";

        public ActionInfo[] SupportedActions { get; } = {
            new()
            {
                ActionName = "По часовой стрелке",
                ActionDescription = "Конвейер будет передвигать объекты по часовой стрелке.",
                ParameterName = "Скорость вращения",
                ValueType = ActionValueType.Float,
                MaxFloatValue = 10
            },
            new()
            {
                ActionName = "Против часовой стрелке",
                ActionDescription = "Конвейер будет передвигать объекты против часовой стрелки.",
                ParameterName = "Скорость вращения",
                ValueType = ActionValueType.Float,
                MaxFloatValue = 10
            },
            new()
            {
                ActionName = "Остановка",
                ActionDescription = "Конвейер не будет двигаться вне зависимости от сигнала."
            }
        };

        public ProgrammableType Type => ProgrammableType.Executor;
        public ActionData SelectedAction { get; set; }
        public void Begin(ActionData action)
        {
            _behaviour = (ConveyorBeltBehaviour)action.ActionIndex;
            _speed = action.ActionIndex != 2 ? (float) action.StoredValue! / 2f : 0f;
            foreach (var child in InnerBelts)
            {
                switch(_behaviour)
                {
                    case ConveyorBeltBehaviour.Clockwise:
                        child.IsCounterClockwise = false;
                        child.ScrollSpeed = _speed;
                        child.Toggle(_enabled);
                        break;
                    case ConveyorBeltBehaviour.CounterClockwise:
                        child.IsCounterClockwise = true;
                        child.ScrollSpeed = _speed;
                        child.Toggle(_enabled);
                        break;
                    case ConveyorBeltBehaviour.Nothing:
                        child.ScrollSpeed = 0f;
                        child.Toggle(_enabled);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }

    public enum ConveyorBeltBehaviour
    {
        Clockwise,
        CounterClockwise,
        Nothing
    }
}