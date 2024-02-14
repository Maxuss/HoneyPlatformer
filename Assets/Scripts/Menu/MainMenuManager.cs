using System;
using DG.Tweening;
using Save;
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
        private Transform title;

        private void Awake()
        {
            aboutContainer.SetActive(false);
            mainContainer.SetActive(true);

            SaveManager.CurrentState.LevelIndex = 13;
            SaveManager.SaveGame();
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
            mainContainer.SetActive(true);
        }
    }
}