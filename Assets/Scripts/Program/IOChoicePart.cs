using System;
using Controller;
using Program.Channel;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Program
{
    public class IOChoicePart: MonoBehaviour, IPointerClickHandler
    {
        [field: SerializeField]
        public IOPreference Preference { get; set; }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                Destroy();
        }

        public void OnPointerClick(PointerEventData e)
        {
            var io = CameraController.Instance.VisualEditing.ChoosingIO;
            if(!CameraController.Instance.VisualEditing.Enabled || io == null)
                return;

            switch (Preference)
            {
                case IOPreference.Input:
                    // we have chosen to connect from input (so starting to connect a rx to tx)
                    var rx = io.GetComponent<IChannelReceiver>();
                    CameraController.Instance.VisualEditing.ConnectingTo = rx;
                    CameraController.Instance.VisualEditing.IsConnecting = true;
                    CameraController.Instance.VisualEditing.SetupConnectingLine(io.transform.position);
                    Destroy();
                    break;
                case IOPreference.Output:
                    // we have chosen to connect output (so starting to connect a tx to rx)
                    var tx = io.GetComponent<IChannelSender>();
                    if (tx.ConnectionLocked) // should not happen at this point, but better safe than sorry
                        return;
                    CameraController.Instance.VisualEditing.ConnectingFrom = tx;
                    CameraController.Instance.VisualEditing.IsConnecting = true;
                    CameraController.Instance.VisualEditing.SetupConnectingLine(io.transform.position);
                    Destroy();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void Destroy()
        {
            CameraController.Instance.VisualEditing.ChoosingIO = null;
            DestroyImmediate(transform.parent.gameObject);
        }
    }
    
    public enum IOPreference
    {
        Input,
        Output
    }
}