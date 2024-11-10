using System;
using System.Linq;
using DG.Tweening;
using UnityEngine;

namespace DonDomain.BG
{
    public class BackgroundChangeHandler: MonoBehaviour
    {
        public static BackgroundChangeHandler Instance { get; private set; }
        
        [SerializeField]
        private SpriteRenderer bg;

        [SerializeField]
        private SpriteRenderer[] rods;
        [SerializeField]
        private SpriteRenderer[] glowCapsules;

        private Material _spriteBg;

        public BackgroundTweener Tweener { get; private set; }

        private void Start()
        {
            _spriteBg = bg.material;
            Tweener = new BackgroundTweener(_spriteBg, rods.Select(it => it.material).ToArray(), glowCapsules.Select(it => it.material).ToArray());
            Instance = this;
        }

        public void SignalEnter(DonBackground self)
        {
            self.Change(Tweener);
        }
    }

    public class BackgroundTweener
    {
        private Material _material;
        private Material[] _rods;
        private Material[] _highlights;
        
        private Tween _spinSpeed;
        private Tween _spinAmount;
        private Tween _contrast;
        private Tween _pixelSizeFactor;
        private Tween _spinEasing;
        private Tween _color1;
        private Tween _color2;
        private Tween _color3;

        private Tween rodAndHighlights;

        private static readonly int OutlineColor = Shader.PropertyToID("_OutlineColor");
        private static readonly int Glow = Shader.PropertyToID("_Glow1");

        private static readonly int SpinSpeed = Shader.PropertyToID("_SpinSpeed");
        private static readonly int SpinAmount = Shader.PropertyToID("_SpinAmount");
        private static readonly int Contrast = Shader.PropertyToID("_Contrast");
        private static readonly int PixelSizeFactor = Shader.PropertyToID("_PixelSizeFactor");
        private static readonly int SpinEasing = Shader.PropertyToID("_SpinEase");
        private static readonly int Color1 = Shader.PropertyToID("_Color1");
        private static readonly int Color2 = Shader.PropertyToID("_Color2");
        private static readonly int Color3 = Shader.PropertyToID("_Color3");
        
        public BackgroundTweener(Material mat, Material[] rods, Material[] highlights)
        {
            _material = mat;
            _rods = rods;
            _highlights = highlights;
        }

        public void DoHighlights(Color newValue, float time = 1f, Ease ease = Ease.Unset)
        {
            if (rodAndHighlights == null)
            {
                var sequence = DOTween.Sequence();
                foreach (var tweenerCore in _rods.Select(it => it.DOColor(newValue, Glow, time).SetEase(ease)))
                {
                    sequence.Insert(0, tweenerCore);
                }

                foreach (var tweener in
                         _highlights.Select(it => it.DOColor(newValue, OutlineColor, time).SetEase(ease)))
                {
                    if(tweener != null)
                        sequence.Insert(0, tweener);
                }

                rodAndHighlights = sequence.Play().OnComplete(() => rodAndHighlights = null);
            }
            else
            {
                var sequence = DOTween.Sequence();
                foreach (var tweenerCore in _rods.Select(it => it.DOColor(newValue, Glow, time).SetEase(ease)))
                {
                    sequence.Insert(0, tweenerCore);
                }

                foreach (var tweener in
                         _highlights.Select(it => it.DOColor(newValue, OutlineColor, time).SetEase(ease)))
                {
                    sequence.Insert(0, tweener);
                }

                rodAndHighlights = rodAndHighlights.OnComplete(() =>
                    rodAndHighlights = sequence.Play().OnComplete(() => rodAndHighlights = null));
            }
        }

        public void DoSpinSpeed(float newValue, float time = 1f, Ease ease = Ease.Unset)
        {
            _QueueFloatTween(val => _spinSpeed = val, _spinSpeed, SpinSpeed, newValue, time, ease);
        }
        
        public void DoSpinAmount(float newValue, float time = 1f, Ease ease = Ease.Unset)
        {
            _QueueFloatTween(val => _spinAmount = val, _spinAmount, SpinAmount, newValue, time, ease);
        }
        
        public void DoContrast(float newValue, float time = 1f, Ease ease = Ease.Unset)
        {
            _QueueFloatTween(val => _contrast = val, _contrast, Contrast, newValue, time, ease);
        }
        
        public void DoPixelSizeFactor(float newValue, float time = 1f, Ease ease = Ease.Unset)
        {
            _QueueFloatTween(val => _pixelSizeFactor = val, _pixelSizeFactor, PixelSizeFactor, newValue, time, ease);
        }
        
        public void DoSpinEasing(float newValue, float time = 1f, Ease ease = Ease.Unset)
        {
            _QueueFloatTween(val => _spinEasing = val, _spinEasing, SpinEasing, newValue, time, ease);
        }
        
        public void DoColor1(Color newValue, float time = 1f, Ease ease = Ease.Unset)
        {
            _QueueColorTween(val => _color1 = val, _color1, Color1, newValue, time, ease);
        }
        
        public void DoColor2(Color newValue, float time = 1f, Ease ease = Ease.Unset)
        {
            _QueueColorTween(val => _color2 = val, _color2, Color2, newValue, time, ease);
        }
        
        public void DoColor3(Color newValue, float time = 1f, Ease ease = Ease.Unset)
        {
            _QueueColorTween(val => _color3 = val, _color3, Color3, newValue, time, ease);
        }

        private void _QueueFloatTween(Action<Tween> setter, Tween self, int propertyId, float newValue, float time, Ease ease)
        {
            if (self == null)
                setter(_material.DOFloat(newValue, propertyId, time).SetEase(ease).Play()
                    .OnComplete(() => setter(null)));
            else
                setter(self.OnComplete(() =>
                {
                    setter(_material.DOFloat(newValue, propertyId, time).SetEase(ease).Play()
                        .OnComplete(() => setter(null)));
                }));
        }

        private void _QueueColorTween(Action<Tween> setter, Tween self, int propertyId, Color newValue, float time,
            Ease ease)
        {
            if (self == null)
                setter(_material.DOColor(newValue, propertyId, time).SetEase(ease).Play()
                    .OnComplete(() => setter(null)));
            else
                setter(self.OnComplete(() =>
                {
                    setter(_material.DOColor(newValue, propertyId, time).SetEase(ease).Play()
                        .OnComplete(() => setter(null)));
                }));
        }
    }
}