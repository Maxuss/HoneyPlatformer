using System;
using TMPro;
using UnityEngine;

namespace Objects.Misc
{
    public class CountdownDisplay: MonoBehaviour
    {

        [SerializeField]
        private float totalCount;
        [SerializeField]
        private float speed = 1f;

        [SerializeField]
        private bool active = true;
        
        public bool IsActive
        {
            get => active;
            set => active = value;
        }

        public float Count
        {
            get => _count;
            set => _count = value;
        }

        [SerializeField]
        [ColorUsage(true, true)]
        private Color beginColor;
        [SerializeField]
        [ColorUsage(true, true)]
        private Color endColor;
        public Action OnReachEnd { get; set; } = () => { };

        private TMP_Text _countText;
        private Material _textMaterial;
        private float _count;
        private static readonly int OutlineColor = Shader.PropertyToID("_OutlineColor");
        private bool _played;


        private void Start()
        {
            _count = totalCount;
            _countText = GetComponentInChildren<TMP_Text>();
            _countText.SetText(((int) _count).ToString());
            _textMaterial = _countText.fontMaterial;
            _textMaterial.SetColor(OutlineColor, beginColor);
        }

        private void Update()
        {
            if (!IsActive)
                return;
            _count -= Time.deltaTime * speed;
            _countText.SetText(((int) Math.Round(_count)).ToString());
            _textMaterial.SetColor(OutlineColor, Color.Lerp(beginColor, endColor, 1 - _count / totalCount));

            if (!(_count <= 0f) || _played) return;
            
            _played = true;
            OnReachEnd();
            IsActive = false;
            _count = 0f;
        }
    }
}