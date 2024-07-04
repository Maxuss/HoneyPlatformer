using System;
using System.Collections.Generic;
using Level;
using Nodes;
using Program;
using Program.Channel;
using UnityEngine;
using Utils;

namespace Objects.Processors
{
    public class BrokenNode: MonoBehaviour, IChannelReceiver, IActionContainer, IChannelSender
    {
        private bool _state;
        private bool _calibrated;
        
        [SerializeField]
        private Transform connectedReceiver;

        [SerializeField]
        private AudioClip successSfx;

        [SerializeField]
        public GameObject terminalPrefab;

        [SerializeField]
        [ColorUsage(true, true)]
        private Color calibratedColor;

        private IChannelReceiver _rx;
        private Renderer _renderer;
        private static readonly int ColorGlow = Shader.PropertyToID("_ColorGlow");

        public bool IsCalibrated => _calibrated;

        public void MarkCalibrated()
        {
            // TODO: maybe some sound here?
            _calibrated = true;
            SfxManager.Instance.Play(successSfx, .4f);
            _rx?.ReceiveBool(transform, _state);
            _renderer.material.SetColor(ColorGlow, calibratedColor);
            NodeManager.Instance.Close();
            ToastManager.Instance.ShowToast("Калибровка успешна!");
        }

        private void Start()
        {
            if (connectedReceiver != null)
                _rx = connectedReceiver.GetComponent<IChannelReceiver>();
            _renderer = GetComponent<Renderer>();
        }

        public void ReceiveBool(Transform source, bool b)
        {
            _state = b;
            if (_calibrated)
                _rx?.ReceiveBool(transform, _state);
        }

        public void ReceiveFloat(Transform source, float v)
        {
            // We don't support floats yet
        }

        public string Name { get; } = "_";
        public string Description { get; } = "_";
        public ActionInfo[] SupportedActions { get; } = Array.Empty<ActionInfo>();
        public ProgrammableType Type { get; } = ProgrammableType.Processor;
        public ActionData SelectedAction { get; set; }
        public void Begin(ActionData action)
        {
            // unreachable
        }

        public List<IChannelReceiver> ConnectedRx => Util.ListOf(_rx);
        public bool ConnectionLocked { get; set; } = true;
        public void Connect(IChannelReceiver rx)
        {
            _rx = rx;
            if(_calibrated)
                _rx.ReceiveBool(transform, _state);
        }

        public void Disconnect()
        {
            _rx = null;
        }
    }
}