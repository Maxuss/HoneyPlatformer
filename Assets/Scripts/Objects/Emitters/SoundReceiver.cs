using System.Collections.Generic;
using Objects.Executors;
using Program;
using Program.Channel;
using UnityEngine;
using Utils;

namespace Objects.Emitters
{
    public class SoundReceiver: MonoBehaviour, IChannelSender, IActionContainer
    {
        public List<IChannelReceiver> ConnectedRx => Util.ListOf(Rx);
        public bool ConnectionLocked { get; set; } = true;
        [SerializeField]
        private Transform connectedReceiver;
        
        public IChannelReceiver Rx;

        public void Impulse()
        {
            Rx.ReceiveBool(transform, true);
        }
        
        private void OnDrawGizmosSelected()
        {
            if (connectedReceiver != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, connectedReceiver.position);
            }
        }
        
        public void Start()
        {
            if(connectedReceiver != null)
                Rx = connectedReceiver.GetComponent<IChannelReceiver>();
        }
        
        public void Connect(IChannelReceiver rx)
        {
            Rx = rx;
        }

        public void Disconnect()
        {
            Rx = null;
        }

        public string Name => "obj.sound_rx.name";
        public string Description => "obj.sound_rx.desc";
        public ActionInfo[] SupportedActions { get; } = new[]
        {
            new ActionInfo
            {
                ActionName = "Активация звуком",
                ActionDescription = "При активации звуком выводит сигнал 1. Сигнал не сохраняется."
            }
        };

        public ProgrammableType Type => ProgrammableType.Emitter;
        public ActionData SelectedAction { get; set; }
        public void Begin(ActionData action)
        {
        }
    }
}