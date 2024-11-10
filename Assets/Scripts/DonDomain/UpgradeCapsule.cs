using System;
using System.Collections;
using DG.Tweening;
using DonDomain.BG;
using I18N;
using Save;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DonDomain
{
    public class UpgradeCapsule: MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer bar;
        [SerializeField]
        private Transform holdText;
        [SerializeField]
        private Transform costText;

        [SerializeField]
        private Transform unlockedBar;

        [SerializeField]
        private Transform unlockedGroup;

        [SerializeField]
        private int cost;

        [SerializeField]
        private DonUpgrade unlockedUpgrade;

        public bool CanDragThrough => _blocked || _unlocked;

        private bool _unlocked;
        
        private Material _mat;

        private Tween _highlightTween;

        private static readonly int HighlightValue = Shader.PropertyToID("_Thickness");
        private static readonly float Duration = 1.6f;
        
        private void Start()
        {
            _mat = GetComponent<SpriteRenderer>().material;
        }

        private IEnumerator UnlockNextGroup()
        {
            if (unlockedGroup == null)
                yield break;
            
            unlockedGroup.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.05f);
            unlockedGroup.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.1f);
            unlockedGroup.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.05f);
            unlockedGroup.gameObject.SetActive(true);
        }

        private void OnEnable()
        {
            CheckConditions();
        }

        private bool _blocked;
        public void CheckConditions()
        {
            if (_unlocked)
                return;
            
            if (SaveManager.CurrentState.Currency < cost)
            {
                _blocked = true;
                costText.GetComponent<LocalizedText>().UpdateText("donmenu.capsule.cost.blocked", cost);
                bar.color = Color.clear;
                holdText.GetComponent<LocalizedText>().UpdateText("donmenu.capsule.hold.blocked");
            }
            else
            {
                costText.GetComponent<LocalizedText>().UpdateText("donmenu.capsule.cost", cost);
            }
        }

        public virtual void OnHover()
        {
            if (_highlightTween == null)
                _highlightTween = _mat.DOFloat(1, HighlightValue, 0.5f).Play().OnComplete(() => _highlightTween = null);
            else
            {
                _highlightTween.OnComplete(() =>
                {
                    _highlightTween = _mat.DOFloat(1, HighlightValue, 0.5f).Play()
                        .OnComplete(() => _highlightTween = null);
                });
            }
        }

        public virtual void OnUnhover()
        {
            if (_highlightTween == null)
                _highlightTween = _mat.DOFloat(0f, HighlightValue, 0.5f).Play().OnComplete(() => _highlightTween = null);
            else
            {
                _highlightTween.OnComplete(() =>
                {
                    _highlightTween = _mat.DOFloat(0f, HighlightValue, 0.5f).Play()
                        .OnComplete(() => _highlightTween = null);
                });
            }
            if (_unlocked || _busy)
                return;
            DonAudioManager.Instance.StopLoad();
            DonShopManager.Instance.EndSwirl();

            _busy = true;

            if (_shakeTweener != null)
            {
                _shakeTweener.Kill();
                _shakeTweener = null;
                transform.position = _originalPosition;
            }
            
            _floatTweener = DOTween.To(() => _holdCounter, v => _holdCounter = v, 0f, 0.5f).OnComplete(() => holdText.gameObject.SetActive(true)).Play();
            _busy = false;
        }

        public virtual void OnClick()
        {
        }

        private bool _busy;

        public virtual void OnUnclick()
        {
            if (_unlocked || _busy)
                return;
            
            DonAudioManager.Instance.StopLoad();
            DonShopManager.Instance.EndSwirl();
            _busy = true;
            if (_shakeTweener != null)
            {
                _shakeTweener.Kill();
                _shakeTweener = null;
                transform.position = _originalPosition;
            }

            _floatTweener = DOTween.To(() => _holdCounter, v => _holdCounter = v, 0f, 0.5f).OnComplete(() => holdText.gameObject.SetActive(true)).Play();
            _busy = false;
        }

        private Vector3 _originalPosition;
        private Tween _shakeTweener;
        private Tween _floatTweener;
        private float _holdCounter;

        public virtual void HoldTick()
        {
            if (_unlocked || _blocked)
                return;

            if (_shakeTweener == null)
            {
                DonAudioManager.Instance.BeginLoad();
                DonShopManager.Instance.BeginSwirl();
                _originalPosition = transform.position;
                _shakeTweener = transform.DOShakePosition(Duration, 0.05f, 50, fadeOut: false).SetEase(Ease.InExpo).OnComplete(() => _shakeTweener = null).Play();
            }
            
            if (_floatTweener != null)
            {
                var v = _holdCounter;
                _floatTweener.Kill();
                _floatTweener = null;
                _holdCounter = v;
            }

            _holdCounter += Time.deltaTime;

            if (!(_holdCounter >= Duration)) return;
            
            MarkUnlocked();
                
            SaveManager.CurrentState.DonUpgrades.Add(unlockedUpgrade);
            SaveManager.SaveGame(true);

            StartCoroutine(UnlockNextGroup());
            StartCoroutine(DonShopManager.Instance.Spend(cost));

            DonAudioManager.Instance.PlayDone();
            DonShopManager.Instance.EndSwirl();
            DonShopManager.Instance.PollConditions();
                
            Finish();
        }

        public void MarkUnlocked()
        {
            bar.transform.parent.gameObject.SetActive(false);
            costText.gameObject.SetActive(false);
            unlockedBar.gameObject.SetActive(true);
            _unlocked = true;
        }

        public virtual void Finish()
        {
            
        }

        private void Update()
        {
            if (_unlocked)
                return;
            bar.size = new Vector2(Mathf.Lerp(0, 3, _holdCounter / Duration), 0.4375f);
            if (_holdCounter > 0)
            {
                if(holdText.gameObject.activeSelf)
                    holdText.gameObject.SetActive(false);
            }
        }
    }
}