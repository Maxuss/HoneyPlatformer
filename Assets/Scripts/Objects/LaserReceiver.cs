using System;
using Program;
using Program.Channel;
using UnityEngine;

namespace Objects
{
    public class LaserReceiver: MonoBehaviour, IActionContainer
    {
        [SerializeField]
        private Transform connectedReceiver;

        [SerializeField]
        private Sprite disabledSprite;
        [SerializeField]
        private Sprite enabledSprite;

        private LaserEmitter.EmitterColor _connectedColor;
        private LaserEmitter.EmitterColor _chosenColor = LaserEmitter.EmitterColor.Red;
        private ReceiverMode _mode = ReceiverMode.OnlyColor;

        private SpriteRenderer _renderer;
        private IChannelReceiver _rx;
        private bool _state;
        private bool _isConnected;

        private void Start()
        {
            _rx = connectedReceiver.GetComponent<IChannelReceiver>();
            _renderer = GetComponent<SpriteRenderer>();
        }

        public void ReceiveLaser(LaserEmitter.EmitterColor laserColor)
        {
            _isConnected = true;
            _connectedColor = laserColor;
            Debug.Log($"RECEIVING LASER {laserColor} {_chosenColor}");
            switch (_mode)
            {
                case ReceiverMode.Any:
                    _state = true;
                    _renderer.sprite = enabledSprite;
                    _rx.ReceiveBool(transform, _state);
                    break;
                case ReceiverMode.OnlyColor when laserColor == _chosenColor:
                    _state = true;
                    _renderer.sprite = enabledSprite;
                    _rx.ReceiveBool(transform, _state);
                    break;
                case ReceiverMode.ExceptColor when laserColor != _chosenColor:
                    _state = true;
                    _renderer.sprite = enabledSprite;
                    _rx.ReceiveBool(transform, _state);
                    break;
                default:
                    _state = false;
                    _renderer.sprite = disabledSprite;
                    _rx.ReceiveBool(transform, _state);
                    break;
            }
        }

        public void Detach()
        {
            _isConnected = false;
            _state = false;
            _rx.ReceiveBool(transform, _state);
            // TODO: sounds?
            _renderer.sprite = disabledSprite;
        }

        public string Name => "Приемник Лазера";
        public string Description => "Выдает сигнал 1 когда в него попадает лазер подходящий под настроенные условия.";

        public ActionInfo[] SupportedActions { get; } =
        {
            new()
            {
                ActionName = "Любой лазер",
                ActionDescription = "Выдает сигнал 1 когда в него попадает любой лазер",
            },
            new()
            {
                ActionName = "Цветной лазер",
                ActionDescription = "Выдает сигнал 1 когда в него попадает лазер определенного цвета",
                ValueType = ActionValueType.Enum,
                EnumType = typeof(LaserEmitter.EmitterColor),
                ParameterName = "Цвет лазера"
            },
            new()
            {
                ActionName = "Цветная маска",
                ActionDescription = "Выдает сигнал 1 когда в него попадает лазер любого цвета кроме определенного",
                ValueType = ActionValueType.Enum,
                EnumType = typeof(LaserEmitter.EmitterColor),
                ParameterName = "Цвет маски"
            }
        };

        public ProgrammableType Type => ProgrammableType.Emitter;
        public ActionData SelectedAction { get; set; }
        public void Begin(ActionData action)
        {
            var mode = (ReceiverMode)Enum.ToObject(typeof(ReceiverMode), action.ActionIndex);
            if (action.StoredValue != null && action.StoredValue != (object) _chosenColor)
            {
                _chosenColor = (LaserEmitter.EmitterColor)action.StoredValue!;
                if(_isConnected)
                    ReceiveLaser(_connectedColor);
            }

            if (mode == _mode)
                return;
            _mode = mode;
            // recalculate
            if(_isConnected)
                ReceiveLaser(_connectedColor);
        }
    }

    public enum ReceiverMode
    {
        Any,
        OnlyColor,
        ExceptColor
    }
}