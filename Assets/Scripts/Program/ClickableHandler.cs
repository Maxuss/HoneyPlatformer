using System;
using System.Collections;
using Controller;
using Program.Channel;
using Program.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Program
{
    public class ClickableHandler: MonoBehaviour, IPointerClickHandler
    {
        private Renderer _renderer;
        private IActionContainer _actionContainer;
        private IChannelSender _tx;
        private IChannelReceiver _rx;

        protected static readonly int ShouldHighlight = Shader.PropertyToID("_ShouldHighlight");
        protected static readonly int HighlightAmount = Shader.PropertyToID("_HighlightAmount");

        protected virtual void Start()
        {
            _renderer = GetComponent<Renderer>();
            _actionContainer = GetComponent<IActionContainer>();
            
            _tx = GetComponent<IChannelSender>();
            _rx = GetComponent<IChannelReceiver>();
        }

        public virtual IEnumerator OnMouseEnter()
        {
            if (!CameraController.Instance.VisualEditing.Enabled)
                yield break;
            
            var amount = 0f;

            _renderer.material.SetFloat(ShouldHighlight, 1f);
            while (amount < 1f)
            {
                amount += 1.4f * Time.deltaTime;
                _renderer.material.SetFloat(HighlightAmount, Mathf.Min(amount, 1f));
                yield return null;
            }
            _renderer.material.SetFloat(HighlightAmount, 1f);
        }

        public  virtual void OnMouseExit()
        {
            _renderer.material.SetFloat(ShouldHighlight, 0f);
            _renderer.material.SetFloat(HighlightAmount, 0f);
        }

        public virtual void OnPointerClick(PointerEventData e)
        {
            if (!CameraController.Instance.VisualEditing.Enabled)
                return;

            if (CameraController.Instance.VisualEditing.Enabled && Input.GetKey(KeyCode.LeftAlt) && _tx != null)
            {
                _tx.Disconnect();
                CameraController.Instance.VisualEditing.FinishConnection();
                return;
            }
            
            switch (e.button)
            {
                case PointerEventData.InputButton.Left when CameraController.Instance.VisualEditing.IsConnecting:
                {
                    if (CameraController.Instance.VisualEditing.ConnectingFrom != null && _rx != null && _rx is not IBiChannelReceiver) // Bichannel receivers currently do not support reconnections
                    {
                        // a sender was clicked first, so we are trying to attach a receiver
                        CameraController.Instance.VisualEditing.ConnectingFrom.Connect(_rx);
                        CameraController.Instance.VisualEditing.FinishConnection();
                    } else if (CameraController.Instance.VisualEditing.ConnectingTo != null && _tx != null)
                    {
                        // a receiver was clicked first so we are trying to attach a sender
                        if (_tx.ConnectionLocked)
                            return;
                        
                        _tx.Connect(CameraController.Instance.VisualEditing.ConnectingTo);
                        CameraController.Instance.VisualEditing.FinishConnection();
                    }

                    break;
                }
                case PointerEventData.InputButton.Left when _actionContainer == null:
                    return;
                case PointerEventData.InputButton.Left:
                    ProgrammableUIManager.Instance.OpenFor(_actionContainer);
                    break;
                case PointerEventData.InputButton.Right:
                {
                    // starting connecting
                    // TODO: mention somewhere in tutorial that if you hold SHIFT it will always select TX only
                    var rxNull = _rx == null;
                    var txNull = _tx == null;
                    if (!rxNull && !txNull && _rx is not IBiChannelReceiver)
                    {
                        if (Input.GetKey(KeyCode.LeftShift))
                        {
                            if (_tx.ConnectionLocked)
                                return;
                            CameraController.Instance.VisualEditing.ConnectingFrom = _tx;
                            CameraController.Instance.VisualEditing.IsConnecting = true;
                            CameraController.Instance.VisualEditing.SetupConnectingLine(transform.position);
                        }
                        else
                        {
                            CameraController.Instance.VisualEditing.ConnectingTo = _rx;
                            CameraController.Instance.VisualEditing.IsConnecting = true;
                            CameraController.Instance.VisualEditing.SetupConnectingLine(transform.position);
                        }
                    }
                    else if (!rxNull && _rx is not IBiChannelReceiver)
                    {
                        CameraController.Instance.VisualEditing.ConnectingTo = _rx;
                        CameraController.Instance.VisualEditing.IsConnecting = true;
                        CameraController.Instance.VisualEditing.SetupConnectingLine(transform.position);
                    } 
                    else if (!txNull)
                    {
                        if (_tx.ConnectionLocked)
                            return;
                        
                        CameraController.Instance.VisualEditing.ConnectingFrom = _tx;
                        CameraController.Instance.VisualEditing.IsConnecting = true;
                        CameraController.Instance.VisualEditing.SetupConnectingLine(transform.position);
                    }

                    break;
                }
                default:
                    return;
            }
        } 
    }
}