using System;
using Controller;
using Objects.Processors;
using Program.UI;
using UnityEngine;

namespace Nodes
{
    public class NodeManager: MonoBehaviour
    {
        public static NodeManager Instance { get; private set; }

        [SerializeField]
        private Transform container;
        
        public BrokenNode SelectedNode { get; set; }

        private GameObject _terminal;
        private bool _isEditing;

        private void Awake()
        {
            Instance = this;
        }

        public void OpenFor(BrokenNode node)
        {
            if (_isEditing)
                return;
            SelectedNode = node;
            _isEditing = true;
            _terminal = Instantiate(node.terminalPrefab, container);
            _terminal.SetActive(true);
            // BuildInitialTerminalMenu();

            CameraController.Instance.VisualEditing.Editing = true;
        }

        public void Close()
        {
            if (!_isEditing)
                return;
            _isEditing = false;
            Destroy(_terminal);
            SelectedNode = null;

            CameraController.Instance.VisualEditing.Editing = false;
        }

        private void Update()
        {
            if (!_isEditing)
                return;

            if (!Input.GetKeyDown(KeyCode.Escape)) return;
            
            Close();
        }
    }
}