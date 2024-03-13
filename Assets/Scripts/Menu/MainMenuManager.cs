using System;
using System.Collections.Generic;
using DG.Tweening;
using Save;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Menu
{
    public class MainMenuManager: MonoBehaviour
    {
        private bool _loading;

        [SerializeField]
        private GameObject aboutContainer;
        [SerializeField]
        private GameObject mainContainer;
        [SerializeField]
        private GameObject loadContainer;
        [SerializeField]
        private Transform title;
        [SerializeField]
        private Transform[] saveContainers;

        private Dictionary<int, SaveState?> _saveStates;

        private void Awake()
        {
            loadContainer.SetActive(false);
            aboutContainer.SetActive(false);
            mainContainer.SetActive(true);
            
            _saveStates = SaveManager.AllSaves();
        }

        private void Start()
        {
            title.transform.DOLocalMoveY(-90f, 1f).Play();
        }

        public void StartGame()
        {
            if (_loading)
                return;
            _loading = true;
            SceneManager.LoadSceneAsync("level_0");
        }

        public void About()
        {
            mainContainer.SetActive(false);
            aboutContainer.SetActive(true);
        }

        public void Main()
        {
            aboutContainer.SetActive(false);
            loadContainer.SetActive(false);
            mainContainer.SetActive(true);
        }

        public void LoadSave(int saveIdx)
        {
            var save = _saveStates[saveIdx];
            if (!save.HasValue)
                return;
            SaveManager.CurrentState = save.Value;
            LevelLoader.Instance.LoadLevel(save.Value.LevelIndex);
        }

        public void LoadGame()
        {
            loadContainer.SetActive(true);
            mainContainer.SetActive(false);

            var idx = 0;
            foreach (var saveContainer in saveContainers)
            {
                var save = _saveStates[idx];
                var bg = saveContainer.GetChild(0); 
                var saveLevel = bg.GetChild(1).GetComponent<TMP_Text>();
                var currency = bg.GetChild(2).GetComponentInChildren<TMP_Text>();
                if (!save.HasValue)
                {
                    saveLevel.text = "Сохранения нет";
                    currency.transform.parent.gameObject.SetActive(false);
                }
                else
                {
                    var sSave = save.Value;
                    saveLevel.text = $"{LevelLoader.LEVEL_NAMES[sSave.LevelIndex - 1]} (УР. {sSave.LevelIndex})";
                    currency.text = sSave.Currency.ToString();
                }
                idx++;
            }
        }
    }
}