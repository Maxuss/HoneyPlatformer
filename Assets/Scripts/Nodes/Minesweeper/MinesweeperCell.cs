using System;
using UnityEngine;
using UnityEngine.UI;

namespace Nodes.Minesweeper
{
    public class MinesweeperCell: MonoBehaviour
    {
        [SerializeField]
        public MinesweeperNode parent;
        [SerializeField]
        private Sprite[] sprites;

        [SerializeField]
        private Sprite flagSprite;

        [SerializeField]
        private Sprite correctSprite;
        [SerializeField]
        private Sprite incorrectSprite;
        
        private Button _btn;
        public MinesweeperNode.CellState state;

        public bool hasFlag;
        
        public void Begin()
        {
            _btn = GetComponent<Button>();
            _btn.onClick.AddListener(OnClick);
        }

        public void SetState(MinesweeperNode.CellState sstate)
        {
            state = sstate;
            _btn.image.sprite = sprites[(int)state];
        }

        private void OnClick()
        {
            if (state == MinesweeperNode.CellState.Empty || state == MinesweeperNode.CellState.Mine)
            {
                if (hasFlag)
                    _btn.image.sprite = sprites[(int)state];
                else
                    _btn.image.sprite = flagSprite;
                hasFlag = !hasFlag;
            }
        }

        public bool CheckCorrect()
        {
            return state == MinesweeperNode.CellState.Mine
                ? hasFlag
                : state != MinesweeperNode.CellState.Empty || !hasFlag;
        }

        public void DisplayCorrect()
        {
            _btn.interactable = false;
            var correct = CheckCorrect();
            if (correct && (int)state == 1)
                _btn.image.sprite = correctSprite;
            else if (!correct)
                _btn.image.sprite = incorrectSprite;
        }
    }
}