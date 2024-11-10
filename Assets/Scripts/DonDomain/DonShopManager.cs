using System;
using System.Collections;
using System.Linq;
using DG.Tweening;
using Save;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace DonDomain
{
    public class DonShopManager:  MonoBehaviour
    {
        public static DonShopManager Instance { get; private set; }

        [SerializeField]
        public int lastScene;

        [SerializeField]
        private Image black;

        [SerializeField]
        private SpriteRenderer bg;

        [SerializeField]
        private TMP_Text counterText;

        [SerializeField]
        private UpgradeCapsule[] capsules;

        [SerializeField]
        private Transform[] groups;
        
        [HideInInspector]
        public float currency = SaveManager.CurrentState.Currency;

        private static readonly int Contrast = Shader.PropertyToID("_Contrast");
        private static readonly int SpinAmount = Shader.PropertyToID("_SpinAmount");

        public void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            
            StartCoroutine(StartRoutine());
            currency = SaveManager.CurrentState.Currency;
            counterText.SetText($"{currency} XP");

            foreach (var upgrade in SaveManager.CurrentState.DonUpgrades)
            {
                UnlockGroups(upgrade);
            }
        }

        private void UnlockGroups(DonUpgrade upgrade)
        {
            switch (upgrade)
            {
                case DonUpgrade.BaseUpgrade:
                    capsules[0].MarkUnlocked();
                    groups[0].gameObject.SetActive(true);
                    break;
                case DonUpgrade.MovSpeed:
                    capsules[1].MarkUnlocked();
                    groups[1].gameObject.SetActive(true);
                    break;
                case DonUpgrade.CamSpeed:
                    capsules[2].MarkUnlocked();
                    groups[3].gameObject.SetActive(true);
                    break;
                case DonUpgrade.JumpHeight:
                    capsules[3].MarkUnlocked();
                    groups[2].gameObject.SetActive(true);
                    break;
                case DonUpgrade.EmPillow:
                    capsules[4].MarkUnlocked();
                    break;
                case DonUpgrade.Hints:
                    capsules[5].MarkUnlocked();
                    groups[4].gameObject.SetActive(true);
                    break;
                case DonUpgrade.FullAccess:
                    capsules[6].MarkUnlocked();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(upgrade), upgrade, null);
            }
        }

        private IEnumerator StartRoutine()
        {
            yield return new WaitForSeconds(0.5f);
            yield return FadeOut();

            yield return new WaitForSeconds(2f);
            yield return ((RectTransform)counterText.transform.parent).DOAnchorPosX(192, 1f).Play().WaitForCompletion();
        }

        public IEnumerator FadeOut()
        {
            yield return black.DOColor(Color.clear, 1f).SetEase(Ease.Linear).Play().WaitForCompletion();
        }
        
        public IEnumerator FadeIn()
        {
            yield return black.DOColor(Color.black, 0.5f).SetEase(Ease.Linear).Play().WaitForCompletion();
        }

        private bool _closing;
        private void Update()
        {
            if (_closing)
                return;
            
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                SaveManager.SaveGame(true);
                StartCoroutine(CloseCoroutine());
            }
        }

        private IEnumerator CloseCoroutine()
        {
            DonAudioManager.Instance.FadeOut();
            yield return FadeIn();
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = false;
            LevelLoader.Instance.LoadLevel(lastScene, true);
        }

        private Tween _swirl;

        public void BeginSwirl()
        {
            _swirl.Kill();
            var seq = DOTween.Sequence();
            seq.Insert(0, bg.material.DOFloat(8f, Contrast, 2f).SetEase(Ease.Linear));
            seq.Insert(0, bg.material.DOFloat(8f, SpinAmount, 2f).SetEase(Ease.Linear));
            _swirl = seq.Play();
        }

        public void EndSwirl()
        {
            if (_swirl == null)
                return;
            _swirl.timeScale = 4f;
            _swirl.SetEase(Ease.InOutExpo);
            _swirl.SmoothRewind();
            _swirl.OnRewind(() => _swirl = null);
        }

        public void PollConditions()
        {
            foreach (var capsule in capsules)
            {
                if(capsule.isActiveAndEnabled)
                    capsule.CheckConditions();
            }
        }

        public IEnumerator Spend(int amount)
        {
            currency = SaveManager.CurrentState.Currency;
            SaveManager.CurrentState.Currency -= amount;
            counterText.SetText($"{currency} XP");
            yield return ((RectTransform)counterText.transform.parent).DOAnchorPosX(0, 0.5f).Play().WaitForCompletion();

            var endVal = currency - amount;
            var shake = ((RectTransform)counterText.transform.parent).DOShakeAnchorPos(10f, 1f, 70, fadeOut: false).Play();
            while (currency > endVal)
            {
                currency -= 200 * Time.deltaTime;
                counterText.SetText($"{Mathf.RoundToInt(currency)} XP");
                yield return null;
            }
            shake.Kill();

            currency = endVal;
            counterText.SetText($"{currency} XP");

            yield return new WaitForSeconds(1f); 
            
            yield return ((RectTransform)counterText.transform.parent).DOAnchorPosX(192, 1f).Play().WaitForCompletion();
        }
    }
}