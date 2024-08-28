using System;
using System.Collections.Generic;
using Controller;
using JetBrains.Annotations;
using Program;
using Program.Channel;
using TMPro;
using UnityEngine;
using Utils;

namespace Objects.Processors
{
    public class UnaryModifier: MonoBehaviour, IChannelReceiver, IActionContainer, IChannelSender
    {
        [SerializeField]
        private UnaryOperator op;

        [SerializeField]
        private Transform connectedReceiver;
        
        private IChannelReceiver _rx;
        private Renderer _renderer;
        
        private bool _state;
        private static readonly int Output = Shader.PropertyToID("_Output");
        private static readonly int InputData = Shader.PropertyToID("_Input");
        
        [SerializeField]
        private TMP_Text visualTextPrefab;

        [CanBeNull] [SerializeField] private Transform visualTextPos;
        
        private GameObject _visualText;

        private void OnEnterVisualMode()
        {
            var text = Instantiate(visualTextPrefab, transform);
            text.text = SelectedAction.ActionIndex == 0 ? "SELF" : "NOT";
            if (visualTextPos != null)
                text.transform.position = visualTextPos.position;
            else
                text.transform.localPosition = new Vector3(0, 1.5f, 0);
            var rot = transform.rotation.eulerAngles;
            text.transform.localRotation = Quaternion.Euler(-rot.x, -rot.y, -rot.z);
            _visualText = text.gameObject;
        }

        private void OnExitVisualMode()
        {
            Destroy(_visualText);
        }

        private void OnDrawGizmosSelected()
        {
            if (connectedReceiver != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, connectedReceiver.position);
            }
        }

        private void Start()
        {
            if(connectedReceiver != null)
                _rx = connectedReceiver.GetComponent<IChannelReceiver>();
            _renderer = GetComponent<Renderer>();
            CameraController.OnEnterMode += OnEnterVisualMode;
            CameraController.OnExitMode += OnExitVisualMode;
        }

        private void OnDestroy()
        {
            CameraController.OnEnterMode -= OnEnterVisualMode;
            CameraController.OnExitMode -= OnExitVisualMode;
        }

        public void ReceiveBool(Transform source, bool b)
        {
            _state = op == UnaryOperator.Identity ? b : !b;
            _renderer.material.SetFloat(InputData, b ? 1f : 0f);
            _renderer.material.SetFloat(Output, _state ? 1f : 0f);
            _rx?.ReceiveBool(transform, _state);
        }
        
        public void ReceiveFloat(Transform source, float v)
        {
            // We don't support floats yet
        }

        public string Name => "obj.unary.name";
        public string Description => "obj.unary.desc";

        public ActionInfo[] SupportedActions => new[]
        {
            new ActionInfo
            {
                ActionName = "Идентичность",
                ActionDescription = "Выводит тот же сигнал, что и был получен на входе."
            },
            new ActionInfo
            {
                ActionName = "Отрицание",
                ActionDescription = "Выводит сигнал, обратный полученному на входе."
            }
        };

        public ProgrammableType Type => ProgrammableType.Processor;
        [field: SerializeField]
        public ActionData SelectedAction { get; set; }
        public void Begin(ActionData action)
        {
            var newOperator = (UnaryOperator) Enum.ToObject(typeof(UnaryOperator), action.ActionIndex);
            if (op == newOperator)
            {
                return;
            }

            op = newOperator;
            _state = !_state;
            _renderer.material.SetFloat(Output, _state ? 1f : 0f);
            _rx?.ReceiveBool(transform, _state);
            _visualText.GetComponent<TMP_Text>().text = SelectedAction.ActionIndex == 0 ? "SELF" : "NOT";
        }

        public List<IChannelReceiver> ConnectedRx => Util.ListOf(_rx);
        [field: SerializeField]
        public bool ConnectionLocked { get; set; }
        public void Connect(IChannelReceiver rx)
        {
            _rx = rx;
            _rx.ReceiveBool(transform, _state);
        }

        public void Disconnect()
        {
            _rx = null;
        }
    }

    public enum UnaryOperator
    {
        Identity,
        Not,
    }
}