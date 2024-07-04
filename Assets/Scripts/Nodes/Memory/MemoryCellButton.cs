using System;
using Nodes.Memory;
using UnityEngine;
using UnityEngine.UI;

namespace Nodes
{
    public class MemoryCellButton: MonoBehaviour
    {
        [SerializeField]
        private MemoryNodeStartButton parent;
        [SerializeField]
        private Sprite[] sprites;
        [SerializeField]
        private int row;
        [SerializeField]
        private int col;

        private MemoryNodeStartButton.ButtonType _state = MemoryNodeStartButton.ButtonType.Empty;
        private Button _button;
        
        private void Start()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(OnClick);
            _button.image.sprite = sprites[3];
        }

        private void OnClick()
        {
            _state = (MemoryNodeStartButton.ButtonType) (((int) _state + 1) % 4);
            _button.image.sprite = sprites[(int) _state];
            parent.NotifyChange(row, col, _state);
        }
    }
}