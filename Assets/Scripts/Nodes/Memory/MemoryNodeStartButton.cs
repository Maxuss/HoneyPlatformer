using System;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Nodes.Memory
{
    public class MemoryNodeStartButton: MonoBehaviour
    {
        [SerializeField]
        private Transform cellBoard;

        [SerializeField]
        private Sprite[] sprites;

        [SerializeField]
        private TMP_Text countdown;
        
        private Button[][] _buttonBoard = { new Button[4], new Button[4], new Button[4], new Button[4] };

        private Button _btn;
        
        private static ButtonType[][][] _variants = new[]
        {
            new[]
            {
                new[] { ButtonType.Empty, ButtonType.Square, ButtonType.Square, ButtonType.Square },
                new[] { ButtonType.Empty, ButtonType.Square, ButtonType.Empty, ButtonType.Empty },
                new[] { ButtonType.Empty, ButtonType.Square, ButtonType.Square, ButtonType.Empty },
                new[] { ButtonType.Cross, ButtonType.Empty, ButtonType.Square, ButtonType.Cross },
            },
            new ButtonType[][]
            {
                new[] { ButtonType.Circle, ButtonType.Empty, ButtonType.Empty, ButtonType.Circle },
                new[] { ButtonType.Empty, ButtonType.Circle, ButtonType.Square, ButtonType.Empty },
                new[] { ButtonType.Empty, ButtonType.Square, ButtonType.Circle, ButtonType.Empty },
                new[] { ButtonType.Circle, ButtonType.Empty, ButtonType.Square, ButtonType.Circle },
            },
            new ButtonType[][]
            {
                new[] { ButtonType.Cross, ButtonType.Circle, ButtonType.Circle, ButtonType.Cross },
                new[] { ButtonType.Empty, ButtonType.Cross, ButtonType.Cross, ButtonType.Empty },
                new[] { ButtonType.Empty, ButtonType.Empty, ButtonType.Empty, ButtonType.Empty },
                new[] { ButtonType.Square, ButtonType.Square, ButtonType.Square, ButtonType.Square },
            },
            new ButtonType[][]
            {
                new[] { ButtonType.Empty, ButtonType.Empty, ButtonType.Cross, ButtonType.Square },
                new[] { ButtonType.Cross, ButtonType.Cross, ButtonType.Empty, ButtonType.Square },
                new[] { ButtonType.Square, ButtonType.Empty, ButtonType.Cross, ButtonType.Cross },
                new[] { ButtonType.Circle, ButtonType.Cross, ButtonType.Empty, ButtonType.Empty },
            },
            new ButtonType[][]
            {
                new[] { ButtonType.Circle, ButtonType.Empty, ButtonType.Square, ButtonType.Empty },
                new[] { ButtonType.Empty, ButtonType.Square, ButtonType.Empty, ButtonType.Empty },
                new[] { ButtonType.Empty, ButtonType.Square, ButtonType.Empty, ButtonType.Empty },
                new[] { ButtonType.Square, ButtonType.Circle, ButtonType.Square, ButtonType.Square },
            },

        };

        private void Start()
        {
            _btn = GetComponent<Button>();
            for (var row = 0; row < 4; row++)
            {
                for (var col = 0; col < 4; col++)
                {
                    _buttonBoard[row][col] = cellBoard.GetChild(row * 4 + col).GetComponent<Button>();
                }
            }

            _chosenMatrix = _variants[Random.Range(0, _variants.Length)];
            
            _btn.onClick.AddListener(Begin);
            Debug.Log("CHOSEN MATRIX");
            Debug.Log(_chosenMatrix);
        }

        private ButtonType[][] _chosenMatrix;
        private ButtonType[][] _currentMatrix =
        {
            Enumerable.Repeat(ButtonType.Empty, 4).ToArray(), 
            Enumerable.Repeat(ButtonType.Empty, 4).ToArray(),
            Enumerable.Repeat(ButtonType.Empty, 4).ToArray(),
            Enumerable.Repeat(ButtonType.Empty, 4).ToArray()
        };

        public void NotifyChange(int row, int col, ButtonType newType)
        {
            _currentMatrix[row][col] = newType;
            Debug.Log(_currentMatrix);

            if (_chosenMatrix.SelectMany(x => x).SequenceEqual(_currentMatrix.SelectMany(x => x)))
            {
                // TODO: mark node as completed
                NodeManager.Instance.SelectedNode.MarkCalibrated();
                Debug.Log("NODE COMPLETED");
            }
            else
            {
                Debug.Log("NODE STILL NOT COMPLETED");
            }
        }

        public void Begin()
        {
            _btn.interactable = false;
            _btn.image.color = Color.gray;

            foreach (var btn in _buttonBoard.SelectMany(x => x))
                btn.interactable = false;
            
            StartCoroutine(Show());
        }
        
        public IEnumerator Show()
        {
            yield return ShowRow(0);
            yield return new WaitForSeconds(0.3f);
            yield return ShowRow(1);
            yield return ShowRow(2);
            yield return ShowRow(3);

            yield return Countdown();
            
            foreach (var btn in _buttonBoard.SelectMany(x => x))
                btn.interactable = true;
        }
        
        private IEnumerator Countdown()
        {
            countdown.gameObject.SetActive(true);

            var count = 5f;
            while (count > 0f)
            {
                count -= Time.deltaTime;
                countdown.text = ((int) Mathf.Round(count)).ToString();
                yield return null;
            }
            
            countdown.gameObject.SetActive(false);
            HideAll();
        }

        private IEnumerator ShowRow(int row)
        {
            // TODO: some sounds?
            var chosenRow = _chosenMatrix[row];
            var btnRow = _buttonBoard[row];
            for (var i = 0; i < 4; i++)
            {
                var btn = btnRow[i];
                btn.image.sprite = sprites[(int)chosenRow[i]];
            }

            yield return new WaitForSeconds(0.4f + Random.Range(-0.1f, 0.1f));
        }

        private void HideAll()
        {
            for (var row = 0; row < 4; row++)
            {
                var r = _buttonBoard[row];
                for (var col = 0; col < 4; col++)
                {
                    r[col].image.sprite = sprites[3];
                }
            }
        }

        public enum ButtonType
        {
            Square,
            Circle,
            Cross,
            Empty
        }
    }
}