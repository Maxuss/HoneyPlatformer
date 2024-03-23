using System;
using System.Collections;
using System.Collections.Generic;
using Program;
using Program.Channel;
using UnityEngine;
using Utils;
using Debug = System.Diagnostics.Debug;

namespace Objects.Processors
{
    public class TimerObject: MonoBehaviour, IChannelReceiver, IActionContainer, IChannelSender
    {
        
        [SerializeField]
        private float selectedTime;
        
        [SerializeField]
        private Transform connectedReceiver;
        
        private IChannelReceiver _rx;
        private bool _state;
        private float _delay = 0f;

        private Coroutine delayedSend;
        
        private void Start()
        {
            if(connectedReceiver != null)
                _rx = connectedReceiver.GetComponent<IChannelReceiver>();
        }
        
        public void ReceiveBool(Transform source, bool b)
        {
            _state = b;
            if (delayedSend != null)
            {
                StopCoroutine(delayedSend);
            }
            delayedSend = StartCoroutine(SendCoroutine());
        }

        private IEnumerator SendCoroutine()
        {
            Debug.Assert(SelectedAction.StoredValue != null, "SelectedAction.StoredValue != null");
            yield return new WaitForSeconds(_delay);
            _rx.ReceiveBool(transform, _state);
            delayedSend = null;
        }

        public void ReceiveFloat(Transform source, float v)
        {
        }

        public string Name => "Таймер";
        public string Description => "Задерживает получаемый сигнал на N секунд";

        public ActionInfo[] SupportedActions { get; } =
        {
            new()
            {
                ActionName = "Задержка",
                ActionDescription = "Получаемый сигнал будет задержан на N секунд",
                ValueType = ActionValueType.Float,
                MaxFloatValue = 10,
                ParameterName = "Время"
            }
        };

        public ProgrammableType Type => ProgrammableType.Processor;

        public ActionData SelectedAction { get; set; }

        public void Begin(ActionData action)
        {
            var newDelay = (float)action.StoredValue!;
            if (Math.Abs(_delay - newDelay) < 0.1)
                return;
            _delay = newDelay;

            if (delayedSend != null)
            {
                StopCoroutine(delayedSend);
                delayedSend = StartCoroutine(SendCoroutine());
            }
        }

        public List<IChannelReceiver> ConnectedRx => Util.ListOf(_rx);
        [field: SerializeField]
        public bool ConnectionLocked { get; set; }
        public void Connect(IChannelReceiver rx)
        {
            _rx = rx;
            if (delayedSend != null)
            {
                StopCoroutine(delayedSend);
            }

            delayedSend = StartCoroutine(SendCoroutine());
        }

        public void Disconnect()
        {
            _rx = null;
            if (delayedSend != null)
            {
                StopCoroutine(delayedSend);
                delayedSend = null;
            }
        }
    }
}