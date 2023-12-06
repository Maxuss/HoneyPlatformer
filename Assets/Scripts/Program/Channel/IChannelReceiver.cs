using UnityEngine;

namespace Program.Channel
{
    public interface IChannelReceiver
    {
        public void ReceiveBool(bool b);
        public void ReceiveFloat(float v);
    }

}