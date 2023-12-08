using System;
using System.Collections.Generic;
using Program;
using Program.Channel;
using UnityEngine;
using Utils;

namespace Controller
{
    public class VisualEditingMode: MonoBehaviour
    {
        [SerializeField]
        private float smootheningModifier = 0.1f;

        [SerializeField] 
        private Transform renderLineContainer;
        [SerializeField]
        private GameObject linePrefab;
        
        private List<LineRenderer> _lines = new();

        public bool Enabled { get; set; }
        public bool Editing { get; set; }

        private Camera _camera;
        private Vector3 _velocity;
        private bool _linesShown;

        // TODO: wire rendering
        private void Start()
        {
            _camera = GetComponent<Camera>();
        }

        private void Update()
        {
            if (!Enabled || Editing)
                return;

            if (Input.GetKeyDown(KeyCode.C))
            {
                if(_linesShown)
                    ClearLines();
                else
                    RenderAllLines();
                _linesShown = !_linesShown;
            }

            var horizontal = Vector2.right * Input.GetAxisRaw("Horizontal");
            var vertical = Vector2.up * Input.GetAxisRaw("Vertical");
            var newPos = transform.position + (Vector3) (horizontal + vertical);
            transform.position = 
                Vector3.SmoothDamp(transform.position, newPos, ref _velocity, smootheningModifier, Mathf.Infinity);
        }

        public void RenderAllLines()
        {
            foreach(var obj in Util.GetAllComponents<IChannelSender>())
            {
                RenderLines(obj);
            }
        }
        
        public void RenderLines(IChannelSender tx)
        {
            var txPos = ((MonoBehaviour)tx).transform.position;
            foreach (var rx in tx.ConnectedRx)
            {
                var rxPos = ((MonoBehaviour)rx).transform.position;
                var line = Instantiate(linePrefab, renderLineContainer).GetComponent<LineRenderer>();
                line.SetPositions(new[] { txPos, rxPos });
                var center = 0.5f * (rxPos - txPos) + txPos;
                var arrow = line.transform.GetChild(0);
                arrow.position = center;
                var quaternion = Quaternion.LookRotation(Vector3.forward, rxPos - txPos);
                quaternion.x = 0;
                quaternion.y = 0;
                arrow.rotation = quaternion;

                var sizeModifier = Mathf.Clamp((rxPos - txPos).sqrMagnitude / 64f, 0.58f, 1f);
                arrow.localScale *= sizeModifier;
                _lines.Add(line);
            }
        }

        public void ClearLines()
        {
            foreach (var line in _lines)
            {
                DestroyImmediate(line.gameObject);
            }
            _lines.Clear();
        }
    }
}