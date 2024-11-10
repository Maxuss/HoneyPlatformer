using System;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DonDomain
{
    public class IndicatorMovement: MonoBehaviour
    {
        private float deltaX;
        private float deltaY;

        private void Start()
        {
            var sequence = DOTween.Sequence();
            sequence.Append(transform.DOLocalMoveY(transform.localPosition.y - 0.5f * Random.Range(0.5f, 2f), 4f * Random.Range(0.9f, 1.1f)).SetEase(Ease.InOutCubic));
            sequence.Append(transform.DOLocalMoveY(transform.localPosition.y, 4f).SetEase(Ease.InOutCubic));
            // sequence.Append(transform.DOLocalMoveY(transform.localPosition.y, 0.5f).SetEase(Ease.Linear));

            sequence.SetLoops(-1).Play();
        }
    }
}