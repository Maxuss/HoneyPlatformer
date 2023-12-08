using System;
using UnityEngine;

namespace Program.Channel
{
    public interface IBiChannelReceiver: IChannelReceiver
    {
        Transform SourceLeft { get; set; }
        Transform SourceRight { get; set; }
        
        void ReceiveBool(MessageDirection direction, Transform src, bool b);
        void ReceiveFloat(MessageDirection direction, Transform src, float v);
        
        void IChannelReceiver.ReceiveBool(Transform source, bool b)
        {
            // two checks are important here so that a different object is not sending messages
            // to this channel
            var dir = source == SourceLeft ? MessageDirection.Left :
                source == SourceRight ? MessageDirection.Right :
                throw new Exception(
                    "Invalid bi-channel-receiver message sender. Bi channel receiver only supports two channels.");
            ReceiveBool(dir, source, b);
        }
        
        void IChannelReceiver.ReceiveFloat(Transform source, float v)
        {
            // ditto
            var dir = source == SourceLeft ? MessageDirection.Left :
                source == SourceRight ? MessageDirection.Right :
                throw new Exception(
                    "Invalid bi-channel-receiver message sender. Bi channel receiver only supports two channels.");
            ReceiveFloat(dir, source, v);
        }
    }

    public enum MessageDirection
    {
        Left,
        Right,
    }
}