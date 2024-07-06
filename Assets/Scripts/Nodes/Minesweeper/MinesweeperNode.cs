using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Level;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using Random = UnityEngine.Random;

namespace Nodes.Minesweeper
{
    public class MinesweeperNode: MonoBehaviour
    {
        [SerializeField]
        private Transform cellGrid;
        [SerializeField]
        private MinesweeperCell cellPrefab;
        [SerializeField]
        private AudioClip incorrectSfx;

        private Button _btn;

        private CellState[] _chosenBoard;
        private MinesweeperCell[] _presentBoard = new MinesweeperCell[25];

        public void Start()
        {
            _btn = GetComponent<Button>();
            
            // generate board
            GenerateBoard();

            StartCoroutine(SpawnIn());

            _btn.onClick.AddListener(Check);
        }

        private void Check()
        {
            _btn.interactable = false;
            _btn.GetComponentInChildren<TMP_Text>().text = "Идет проверка...";
            StartCoroutine(CheckCoroutine());
        }

        private IEnumerator CheckCoroutine()
        {
            foreach (var cell in _presentBoard)
            {
                cell.DisplayCorrect();
                if(cell.state == CellState.Mine || cell.state == CellState.Empty && cell.hasFlag)
                    yield return new WaitForSeconds(0.15f + Random.Range(-0.05f, 0.02f));
            }

            yield return new WaitForSeconds(1f);

            if (_presentBoard.All(x => x.CheckCorrect()))
            {
                NodeManager.Instance.SelectedNode.MarkCalibrated();
                
            }
            else
            {
                NodeManager.Instance.Close();
                SfxManager.Instance.Play(incorrectSfx);
                ToastManager.Instance.ShowToast("Калибровка не удалась!");
            }
        }

        private IEnumerator SpawnIn()
        {
            var i = 0;
            foreach (var cell in _chosenBoard)
            {
                var newCell = Instantiate(cellPrefab, cellGrid);
                newCell.parent = this;
                newCell.Begin();
                newCell.SetState(cell);
                _presentBoard[i] = newCell;
                i++;
                if(cell != CellState.Empty && cell != CellState.Mine)
                    yield return new WaitForSeconds(0.09f + Random.Range(-0.05f, 0.02f));
            }
        }

        private void GenerateBoard()
        {
            var mineCount = Random.Range(2, 4);
            
            // var boardMatrix = Enumerable.Repeat(Enumerable.Repeat(CellState.Empty, 5).ToArray(), 5).ToArray();
            var boardMatrix = new[]
            {
                new[] { CellState.Empty, CellState.Empty, CellState.Empty, CellState.Empty, CellState.Empty },
                new[] { CellState.Empty, CellState.Empty, CellState.Empty, CellState.Empty, CellState.Empty },
                new[] { CellState.Empty, CellState.Empty, CellState.Empty, CellState.Empty, CellState.Empty },
                new[] { CellState.Empty, CellState.Empty, CellState.Empty, CellState.Empty, CellState.Empty },
                new[] { CellState.Empty, CellState.Empty, CellState.Empty, CellState.Empty, CellState.Empty }
            };
            var mineCoords = new List<(int, int)>();

            while (mineCount > 0)
            {
                // find a random position where to place mine
                int x = Random.Range(0, 5), y = Random.Range(0, 5);
                Debug.Log($"{x} {y} {boardMatrix.Length}");
                while (boardMatrix[x][y] != CellState.Empty)
                {
                    x = Random.Range(0, 6);
                    y = Random.Range(0, 6);
                }

                boardMatrix[x][y] = CellState.Mine;
                mineCoords.Add((x, y));
                mineCount--;
            }
            
            foreach (var row in boardMatrix)
            {
                var st = new StringBuilder();
                foreach (var col in row)
                {
                    st.Append(col);
                    st.Append(' ');
                }

                Debug.Log(st);
            }

            var coordsValues = new Dictionary<(int, int), int>();
            foreach (var (x, y) in mineCoords)
            {
                var neighbors = new[]
                {
                    (x - 1, y), (x - 1, y + 1), (x, y - 1), (x + 1, y - 1), (x + 1, y), (x + 1, y + 1), (x, y + 1),
                    (x - 1, y - 1)
                };
                Debug.Log(neighbors);
                foreach (var (nx, ny) in neighbors)
                {
                    if (nx >= 0 && nx < 5 && ny >= 0 && ny < 5 && !mineCoords.Contains((nx, ny)))
                    {
                        if (coordsValues.ContainsKey((nx, ny)))
                            coordsValues[(nx, ny)] += 1;
                        else
                            coordsValues[(nx, ny)] = 1;
                    }
                }
            }

            foreach (var ((x, y), val) in coordsValues)
            {
                boardMatrix[x][y] = val switch
                {
                    1 => CellState.One,
                    2 => CellState.Two,
                    3 => CellState.Three,
                    4 => CellState.Four,
                    _ => throw new ArgumentOutOfRangeException()
                };
            }

            
            _chosenBoard = boardMatrix.SelectMany(x => x).ToArray();
        }
        
        public enum CellState
        {
            Empty,
            Mine,
            One,
            Two,
            Three,
            Four
        }
    }
}