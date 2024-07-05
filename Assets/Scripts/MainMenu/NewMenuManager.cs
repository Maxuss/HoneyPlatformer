using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using Save;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace MainMenu
{
    public class NewMenuManager: MonoBehaviour
    {
        [SerializeField]
        private RectTransform mainContainer;
        [SerializeField]
        private RectTransform infoContainer;
        [SerializeField]
        private RectTransform loadContainer;
        [SerializeField]
        private RectTransform logo;
        [SerializeField]
        private Image black;

        [SerializeField]
        private Transform normalCameraPos;
        [SerializeField]
        private Transform infoCameraPos;
        [SerializeField]
        private Transform loadCameraPos;
        [SerializeField]
        private Transform beginCameraPos;

        [SerializeField]
        private ParticleSystem particleSystem;

        [SerializeField]
        private Transform camera;

        [SerializeField]
        private SpriteRenderer[] areas;
        
        private Dictionary<int, SaveState?> _saveStates;
        private AudioSource _as;

        private void Start()
        {
            _saveStates = SaveManager.AllSaves();
            ReloadSaves();
            _as = GetComponent<AudioSource>();
            _as.DOFade(1f, 2.5f);
        }

        private void ReloadSaves()
        {
            var idx = 0;
            var containerInner = loadContainer.GetChild(0);
            var containers = new[]
                { containerInner.GetChild(0), containerInner.GetChild(1), containerInner.GetChild(2) };
            
            foreach (var saveContainer in containers)
            {
                var save = _saveStates[idx];
                var saveLevel = saveContainer.GetChild(0).GetComponent<TMP_Text>();
                var currency = saveContainer.GetChild(1).GetComponent<TMP_Text>();
                if (!save.HasValue)
                {
                    saveContainer.GetComponent<Button>().interactable = false;
                    saveLevel.text = "Сохранения нет";
                    currency.transform.parent.gameObject.SetActive(false);
                }
                else
                {
                    var sSave = save.Value;

                    saveLevel.text = LevelLoader.LEVEL_NAMES[sSave.LevelIndex - 1];
                    currency.text = sSave.Currency.ToString();
                }

                idx++;
            }
        }

        public void LoadSave(int idx)
        {
            var save = _saveStates[idx];
            if (!save.HasValue)
                return;
            SaveManager.CurrentState = save.Value;
            loadContainer.DOAnchorPosX(0f, .5f); 
            camera.DOMove(beginCameraPos.position, 4f);
            camera.DORotate(beginCameraPos.eulerAngles, 4f);
            StartCoroutine(this.CallbackCoroutine(FadeOut(), () => LevelLoader.Instance.LoadLevel(save.Value.LevelIndex)));
        }

        public void HighlightSave(int idx)
        {
            var save = _saveStates[idx];
            if (!save.HasValue)
                return;
            var img = areas[(int)LevelLoader.LEVEL_LOCS[save.Value.LevelIndex - 1]];
            img.DOColor(new Color(1f, 1f, 1f, 1f), .5f);
            
        }

        public void UnhighlightSave(int idx)
        {
            var save = _saveStates[idx];
            if (!save.HasValue)
                return;
            var img = areas[(int)LevelLoader.LEVEL_LOCS[save.Value.LevelIndex - 1]];
            img.DOColor(new Color(1f, 1f, 1f, 0f), .5f);
        }

        public void StartGame(int level)
        {
            var main = particleSystem.main;
            main.startSpeed = new ParticleSystem.MinMaxCurve(-20f);
            logo.DOAnchorPosY(60f, .5f);
            mainContainer.DOAnchorPosX(-180, .5f);
            camera.DOMove(beginCameraPos.position, 4f);
            camera.DORotate(beginCameraPos.eulerAngles, 4f);
            StartCoroutine(this.CallbackCoroutine(FadeOut(), () => LevelLoader.Instance.LoadLevel(level)));
        }

        private IEnumerator FadeOut()
        {
            var cnt = 3f;
            var opacity = 0f;
            while (cnt > 0f)
            {
                cnt -= Time.deltaTime;
                opacity = 1 - (cnt / 3f);
                black.color = new Color(0f, 0f, 0f, opacity);
                yield return null;
            }

            black.color = new Color(0f, 0f, 0f, 1f);
            
        }

        public void InfoSection()
        {
            logo.DOAnchorPosY(60f, .5f);
            var tweener = mainContainer.DOAnchorPosX(-180, .5f);
            tweener.OnComplete(() => infoContainer.DOAnchorPosY(-275f, 1f)); // TO: 225f
            MoveToInfo();
        }

        public void LoadSection()
        {
            logo.DOAnchorPosY(60f, .5f);
            var tweener = mainContainer.DOAnchorPosX(-180, .5f);
            tweener.OnComplete(() => loadContainer.DOAnchorPosX(-400f, 1f)); // TO: 225f
            MoveToLoad();
        }

        public void Info2Main()
        {
            var tweener = infoContainer.DOAnchorPosY(275f, .5f); 
            tweener.OnComplete(() =>
            {
                mainContainer.DOAnchorPosX(0, 1f);
                logo.DOAnchorPosY(-60f, 1f);
            });
            MoveToMain();
        }

        public void Load2Main()
        {
            var tweener = loadContainer.DOAnchorPosX(0f, .5f); 
            tweener.OnComplete(() =>
            {
                mainContainer.DOAnchorPosX(0, 1f);
                logo.DOAnchorPosY(-60f, 1f);
            });
            MoveToMain();
        }

        public void MoveToInfo()
        {
            var main = particleSystem.main;
            main.startSpeed = new ParticleSystem.MinMaxCurve(-20f);
            camera.DOMove(infoCameraPos.position, 1f);
            camera.DORotate(infoCameraPos.eulerAngles, 1f).OnComplete(() =>
            {
                var nMain = particleSystem.main;
                nMain.startSpeed = new ParticleSystem.MinMaxCurve(-0.02f);
            });
        }

        public void MoveToMain()
        {
            var main = particleSystem.main;
            main.startSpeed = new ParticleSystem.MinMaxCurve(-20f);
            camera.DOMove(normalCameraPos.position, 1f);
            camera.DORotate(normalCameraPos.eulerAngles, 1f).OnComplete(() =>
            {
                var nMain = particleSystem.main;
                nMain.startSpeed = new ParticleSystem.MinMaxCurve(-0.02f);
            });
        }

        public void MoveToLoad()
        {
            var main = particleSystem.main;
            main.startSpeed = new ParticleSystem.MinMaxCurve(-20f);
            camera.DOMove(loadCameraPos.position, 1f);
            camera.DORotate(loadCameraPos.eulerAngles, 1f).OnComplete(() =>
            {
                var nMain = particleSystem.main;
                nMain.startSpeed = new ParticleSystem.MinMaxCurve(-0.02f);
            });
        }
    }
}