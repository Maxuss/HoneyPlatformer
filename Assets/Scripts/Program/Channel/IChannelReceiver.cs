using System.Collections.Generic;
using UnityEngine;

namespace Program.Channel
{
    public interface IChannelReceiver
    {
        public void ReceiveBool(Transform source, bool b);
        public void ReceiveFloat(Transform source, float v);
    }

    public interface IChannelSender
    {
        public List<IChannelReceiver> ConnectedRx { get; }
    }
}