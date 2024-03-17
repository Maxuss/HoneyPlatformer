using System;
using System.Collections.Generic;
using Program;
using Program.Channel;
using UnityEngine;
using Utils;

namespace Objects
{
    public class ExposedWires: MonoBehaviour, IChannelSender, IActionContainer
    {
        [SerializeField]
        private Transform connectedReceiver;
        
        private IChannelReceiver _rx;
        private bool _state = false;
        private Transform _ps;
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

        private void Start()
        {
            if(connectedReceiver != null)
                _rx = connectedReceiver.GetComponent<IChannelReceiver>();
            
            _ps = transform.GetChild(0);
        }

        public string Name => "Открытая проводка";
        public string Description => "Крайне уязвимая часть проводки. Кибер-осы будут её ломать, вызывая помехи в сети";

        public ActionInfo[] SupportedActions { get; } = new[]
        {
            new ActionInfo
            {
                ActionName = "Уязвимость",
                ActionDescription = "Если поблизости находится кибер-оса, то постоянно выводит сигнал 0."
            }
        };

        public ProgrammableType Type => ProgrammableType.Emitter;
        public ActionData SelectedAction { get; set; }
        public void Begin(ActionData action)
        {
            // noop
        }

        public void BeeLeaving()
        {
            _state = true;
            _rx.ReceiveBool(transform, true);
            _ps.gameObject.SetActive(false);
        }
    }
}