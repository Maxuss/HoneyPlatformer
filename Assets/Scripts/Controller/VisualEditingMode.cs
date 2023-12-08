using System;
using System.Collections.Generic;
using DG.Tweening;
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
        [SerializeField]
        private List<Gradient> lineColors;
        [SerializeField]
        private RectTransform moreVisualEditingNotifier;
        [SerializeField]
        private RectTransform visualEditingNotifier;

        private bool _tipsHidden;
        private List<LineRenderer> _lines = new();
        private LineRenderer _connectingLine;

        public bool Enabled { get; set; }
        public bool Editing { get; set; }
        public bool IsConnecting { get; set; }
        /// <summary>
        /// For when a sender was clicked first
        /// </summary>
        public IChannelSender ConnectingFrom { get; set; }
        /// <summary>
        /// For when a receiver was clicked first
        /// </summary>
        public IChannelReceiver ConnectingTo { get; set; }

        private Camera _camera;
        private Vector3 _velocity;
        private bool _linesShown;

        private void Start()
        {
            _camera = GetComponent<Camera>();
        }

        private void Update()
        {
            if (!Enabled || Editing)
                return;

            if (IsConnecting)
            {
                _connectingLine.SetPosition(1, _camera.ScreenToWorldPoint(Input.mousePosition));

                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    FinishConnection();
                }
            }

            if (Input.GetKeyDown(KeyCode.X))
            {
                if (_tipsHidden)
                {
                    moreVisualEditingNotifier.DOAnchorPos(new Vector2(50f, -58f), .5f);
                    visualEditingNotifier.DOAnchorPos(new Vector2(50f, 50f), .5f);
                }
                else
                {
                    moreVisualEditingNotifier.DOAnchorPos(new Vector2(-142f, -58f), .5f);
                    visualEditingNotifier.DOAnchorPos(new Vector2(-142f, 50f), .5f);
                }

                _tipsHidden = !_tipsHidden;
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                if (_linesShown)
                {
                    moreVisualEditingNotifier.DOAnchorPos(new Vector2(-142f, -58f), .5f);

                    ClearLines();
                }
                else
                {
                    moreVisualEditingNotifier.gameObject.SetActive(true);
                    moreVisualEditingNotifier.DOAnchorPos(new Vector2(50f, -58f), .5f);
                    RenderAllLines();
                }

                _linesShown = !_linesShown;
            }

            var horizontal = Vector2.right * Input.GetAxisRaw("Horizontal");
            var vertical = Vector2.up * Input.GetAxisRaw("Vertical");
            var newPos = transform.position + (Vector3) (horizontal + vertical);
            transform.position = 
                Vector3.SmoothDamp(transform.position, newPos, ref _velocity, smootheningModifier, Mathf.Infinity);
        }

        public void SetupConnectingLine(Vector3 start)
        {
            _connectingLine = Instantiate(linePrefab, renderLineContainer).GetComponent<LineRenderer>();
            DestroyImmediate(_connectingLine.transform.GetChild(0).gameObject);
            DestroyImmediate(_connectingLine.transform.GetChild(0).gameObject);
            _connectingLine.positionCount = 2;
            _connectingLine.SetPosition(0, start);
        }

        public void FinishConnection()
        {
            ConnectingFrom = null;
            ConnectingTo = null;
            IsConnecting = false;

            if (_connectingLine != null)
            {
                DestroyImmediate(_connectingLine.gameObject);
                _connectingLine = null;
            }

            // re-rendering all lines
            // (we can probably optimize this further by only re-rendering
            // changed lines but im tired)
            _linesShown = true;
            if(!_tipsHidden)
                moreVisualEditingNotifier.DOAnchorPos(new Vector2(-142f, -58f), .5f).OnComplete(() => moreVisualEditingNotifier.gameObject.SetActive(false));
            ClearLines();
            RenderAllLines();
        }

        private void RenderAllLines()
        {
            var groupId = 0;
            foreach(var obj in Util.GetAllComponents<IChannelSender>())
            {
                groupId %= lineColors.Count;
                RenderLine(obj, lineColors[groupId]);
                groupId++;
            }
        }

        private void RenderLine(IChannelSender tx, Gradient newGradient)
        {
            var txPos = ((MonoBehaviour)tx).transform.position;
            foreach (var rx in tx.ConnectedRx)
            {
                if (rx == null)
                    continue;
                var rxPos = ((MonoBehaviour)rx).transform.position;
                var line = Instantiate(linePrefab, renderLineContainer).GetComponent<LineRenderer>();

                line.colorGradient = newGradient;
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

                if (!tx.ConnectionLocked)
                    DestroyImmediate(line.transform.GetChild(1).gameObject);
                else
                {
                    var lockPos = line.transform.GetChild(1);
                    lockPos.position = 0.7f * (rxPos - txPos) + txPos;
                }

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