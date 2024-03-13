using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Level
{
    public class ToastManager: MonoBehaviour
    {
        [SerializeField]
        private RectTransform toast;
        [SerializeField]
        private TMP_Text toastText;
        
        public static ToastManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        public void ShowToast(string text)
        {
            toastText.text = text;
            toast.DOAnchorPos(new Vector2(-120f, -47.5f), 1f);
            toast.DOAnchorPos(new Vector2(120f, -47.5f), 1f).SetDelay(4f);
        }

        public void HideToast()
        {
            toast.DOAnchorPos(new Vector2(-50f, -63f), 1f);
        }
    }
}