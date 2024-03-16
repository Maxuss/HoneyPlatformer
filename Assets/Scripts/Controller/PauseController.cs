using System;
using Level;
using Save;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Controller
{
    public class PauseController: MonoBehaviour
    {
        [SerializeField]
        private GameObject pauseMenu;
        [SerializeField]
        private GameObject saveChoice;
        
        public static bool IsPaused { get; private set; }
        public PauseController Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        private void Update()
        {
            if (!TerminalManager.Instance.InTerminal && !VisualEditingMode.Instance.Enabled)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    if(IsPaused)
                        Unpause();
                    else
                        Pause();
                }
            }
        }

        public void Pause()
        {
            IsPaused = true;
            pauseMenu.SetActive(true);
            Time.timeScale = 0f;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        public void Unpause()
        {
            IsPaused = false;
            pauseMenu.SetActive(false);
            saveChoice.SetActive(false);
            Time.timeScale = 1f;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        public void ChooseSave()
        {
            saveChoice.SetActive(true);
        }

        public void Save(int idx)
        {
            SaveManager.CurrentState.SaveIndex = idx;
            SaveManager.CurrentState.LevelIndex = SceneManager.GetActiveScene().buildIndex;
            SaveManager.SaveGame();
            Unpause();
            ToastManager.Instance.ShowToast($"Игра сохранена в слот {idx + 1}");
        }

        public void Quit()
        {
            Application.Quit();
        }
    }
}