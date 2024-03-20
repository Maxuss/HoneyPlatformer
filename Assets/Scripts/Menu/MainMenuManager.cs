using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using DG.Tweening;
using Save;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Utils;

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
        private GameObject loginContainer;
        [SerializeField]
        private TMP_InputField username;
        [SerializeField]
        private TMP_InputField password;
        [SerializeField]
        private Transform failedToLogin;
        [SerializeField]
        private Transform title;
        [SerializeField]
        private Transform[] saveContainers;
        [SerializeField]
        private Transform[] cloudUploadButtons;
        [SerializeField]
        private Transform[] cloudReplaceButton;
        [SerializeField]
        private GameObject confirmation;
        [SerializeField]
        private TMP_Text toastText;
        [SerializeField]
        private Transform cloudSaveContainer;

        private Dictionary<int, SaveState?> _saveStates;
        private SaveState? _cloudSave;
        private AudioSource _as;

        private void Awake()
        {
            loadContainer.SetActive(false);
            aboutContainer.SetActive(false);
            loginContainer.SetActive(false);
            
            _saveStates = SaveManager.AllSaves();
            _cloudSave = SaveManager.LoadCloud();

            _as = GetComponent<AudioSource>();
        }

        private void Start()
        {
            if (ApiManager.Instance.LoggedIn)
            {
                MainBegin();
            }
            else
            {
                mainContainer.SetActive(false);
                loginContainer.SetActive(true);
            }
        }

        private void MainBegin()
        {
            loginContainer.SetActive(false);
            mainContainer.SetActive(true);
            
            title.transform.DOLocalMoveY(-110f, 1f).Play();
            StartCoroutine(AudioFadeIn());
        }

        private IEnumerator AudioFadeIn()
        {
            _as.volume = 0f;
            _as.Play();
            while (_as.volume < 1)
            {
                _as.volume += .1f;
                yield return new WaitForSeconds(.5f);
            }
        }

        public void TryLogin()
        {
            StartCoroutine(LoginCoroutine());
        }

        private IEnumerator LoginCoroutine()
        {
            yield return ApiManager.Instance.Login(username.text, password.text);

            if (!ApiManager.Instance.LoggedIn)
            {
                failedToLogin.DOLocalMoveX(280, 1f);

                yield return new WaitForSeconds(3f);
                failedToLogin.DOLocalMoveX(520, 1f);
            }
            else
            {
                MainBegin();
            }
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

            ReloadSaves();
        }

        private void ReloadSaves()
        {
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
            
            var cBg = cloudSaveContainer;
            var cSaveLevel = cBg.GetChild(1).GetComponent<TMP_Text>();
            var cCurrency = cBg.GetChild(2).GetComponentInChildren<TMP_Text>(); 
            if (!_cloudSave.HasValue)
            { 
                cSaveLevel.text = "НЕТ";
                cCurrency.transform.parent.gameObject.SetActive(false);
            }
            else
            {
                var sSave = _cloudSave.Value; 
                cSaveLevel.text = $"УР. {sSave.LevelIndex}"; 
                cCurrency.text = sSave.Currency.ToString();
            }
        }

        public void ConfirmationPreReplace(int saveIdx)
        {
            confirmation.transform.GetChild(0).GetComponent<TMP_Text>().text = $"Вы уверены что хотите заменить сохранение в слоте {saveIdx} на облачное?";
            var btn = confirmation.transform.GetChild(1).GetComponent<UnityEngine.UI.Button>();
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => StartCoroutine(ReplaceWithCloudSave(saveIdx)));

            confirmation.SetActive(true);
        }

        public void ConfirmationPreUpload(int saveIdx)
        {
            confirmation.transform.GetChild(0).GetComponent<TMP_Text>().text = $"Вы уверены что загрузить сохранение в слоте {saveIdx} в облако?";
            var btn = confirmation.transform.GetChild(1).GetComponent<UnityEngine.UI.Button>();
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => StartCoroutine(UploadToCloud(saveIdx)));
            confirmation.SetActive(true);
        }

        public IEnumerator ReplaceWithCloudSave(int saveIdx)
        {
            confirmation.SetActive(false);
            yield return SaveManager.ReplaceWithCloud(saveIdx);
            _saveStates = SaveManager.AllSaves(); // TODO: can actually be heavily optimized
            _cloudSave = SaveManager.LoadCloud();
            ReloadSaves();
            toastText.text = $"Сохранение {saveIdx + 1} заменено на облачное";
            toastText.transform.parent.DOLocalMoveX(280, 1);
            StartCoroutine(Util.Delay(() => toastText.transform.parent.DOLocalMoveX(520, 1), 3f));
        }

        public IEnumerator UploadToCloud(int saveIdx)
        {
            confirmation.SetActive(false);
            yield return SaveManager.UploadToCloud(saveIdx);
            _saveStates = SaveManager.AllSaves(); // TODO: same here
            _cloudSave = SaveManager.LoadCloud();
            ReloadSaves();
            toastText.text = $"Сохранение {saveIdx + 1} заменено на облачное";
            var parent = toastText.transform.parent;
            parent.DOLocalMoveX(280, 1);
            yield return new WaitForSeconds(3f);
            parent.DOLocalMoveX(520, 1);
        }
    }
}