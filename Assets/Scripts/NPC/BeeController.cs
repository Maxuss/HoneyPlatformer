using System;
using System.Linq;
using DG.Tweening;
using Objects;
using Objects.Emitters;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

namespace NPC
{
    public class BeeController: MonoBehaviour
    {
        [SerializeField]
        private float radius = 1.5f;
        [SerializeField]
        private float rotationSpeed = 120f;
        [SerializeField]
        private ExposedWires ravagedWires;

        private SpriteRenderer _sr;
        private float _rotationCycle;
        private float _escapeCycle;
        private Vector3 _center;
        private bool _escaping;

        private Vector3[] _stopPositions;
        private Quaternion[] _rotations;
        private Sequence _tween;

        private Animator _animator;

        public bool Active => !_escaping;

        private void Start()
        {
            _animator = GetComponent<Animator>();
            _animator.StopPlayback();
            _animator.Play("Bee", 0, Random.value);
            _center = transform.position;
            _sr = GetComponent<SpriteRenderer>();
            _stopPositions = new[]
            {
                Random.insideUnitCircle.ToVec3(0f) * 2.5f, Random.insideUnitCircle.ToVec3(0f) * 2.5f,
                Random.insideUnitCircle.ToVec3(0f) * 2.5f
            };

            var tf = transform;
            
            var s = DOTween.Sequence();
            foreach (var pos in _stopPositions)
            {
                s.Append(tf.DOMove(_center + pos, 1).SetDelay(0.3f + Util.Rng.Next(10, 50) / 100f).SetEase(Ease.InOutCubic).OnComplete(() =>
                {
                    _sr.flipX = pos.x > 0;
                }));
            }

            s.Append(tf.DOMove(_center, 1).SetEase(Ease.InOutCubic));
            _tween = s.SetRecyclable().OnComplete(() => s.Restart()).Play();

        }

        public void FixedUpdate()
        {
            if (_escaping)
            {
                _tween.Kill();
                _escapeCycle += 2 * Time.fixedDeltaTime;
                transform.position += 0.025f * Mathf.Max(_escapeCycle, 8f) * new Vector3(0, 1f);
            }
        }
        
        public void Escape()
        {
            _escaping = true;
            ravagedWires.BeeLeaving();
        }
    }
}