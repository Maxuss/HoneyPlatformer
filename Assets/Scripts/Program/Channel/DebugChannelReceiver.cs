using UnityEngine;

namespace Program.Channel
{
    
#if UNITY_EDITOR || DEVELOPMENT_MODE
    public class DebugChannelReceiver: MonoBehaviour, IChannelReceiver {
        public void ReceiveBool(bool b)
        {
            Debug.Log($"RECEIVED BOOLEAN STATE {b}");
        }

        public void ReceiveFloat(float v)
        {
            Debug.Log($"RECEIVED FLOAT STATE {v}");
        }
    }
#endif

}