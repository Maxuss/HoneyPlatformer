using UnityEngine;
using UnityEngine.SceneManagement;

namespace Menu
{
    public class MainMenuManager: MonoBehaviour
    {
        private bool _loading;
        public void StartGame()
        {
            if (_loading)
                return;
            _loading = true;
            SceneManager.LoadSceneAsync("level_0");
        }
    }
}