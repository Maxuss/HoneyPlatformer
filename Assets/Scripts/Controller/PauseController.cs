using System;
using Level;
using Save;
using UnityEngine;

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
        }

        public void Unpause()
        {
            IsPaused = false;
            pauseMenu.SetActive(false);
            Time.timeScale = 1f;
        }

        public void ChooseSave()
        {
            saveChoice.SetActive(true);
        }

        public void Save(int idx)
        {
            saveChoice.SetActive(false);
            SaveManager.CurrentState.SaveIndex = idx;
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