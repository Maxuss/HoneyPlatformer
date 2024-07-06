using System;
using Controller;
using Objects.Processors;
using Program.UI;
using UnityEngine;
using UnityEngine.Serialization;

namespace Nodes
{
    public class NodeManager: MonoBehaviour
    {
        public static NodeManager Instance { get; private set; }

        [SerializeField]
        private Transform container;
        
        public BrokenNode SelectedNode { get; set; }

        private GameObject _terminal;
        [FormerlySerializedAs("_isEditing")] public bool isEditing;

        private void Awake()
        {
            Instance = this;
        }

        public void OpenFor(BrokenNode node)
        {
            if (isEditing)
                return;
            SelectedNode = node;
            isEditing = true;
            _terminal = Instantiate(node.terminalPrefab, container);
            _terminal.SetActive(true);
            // BuildInitialTerminalMenu();

            CameraController.Instance.VisualEditing.Editing = true;
        }

        public void Close()
        {
            if (!isEditing)
                return;
            isEditing = false;
            Destroy(_terminal);
            SelectedNode = null;

            CameraController.Instance.VisualEditing.Editing = false;
        }

        private void Update()
        {
            if (!isEditing)
                return;

            if (!Input.GetKeyDown(KeyCode.Escape)) return;
            
            Close();
        }
    }
}