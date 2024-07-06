using System;
using System.Collections;
using System.Numerics;
using UnityEngine;
using Utils;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace NPC
{
    [RequireComponent(typeof(Animator), typeof(SpriteRenderer), typeof(Rigidbody2D))]
    public class NpcController: MonoBehaviour
    {
        private Animator _animator;
        private SpriteRenderer _sr;
        private Rigidbody2D _rb;

        [SerializeField]
        private bool animate = true;
        
        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _sr = GetComponent<SpriteRenderer>();
            _rb = GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            if(animate)
                _animator.Play("Stand");
        }

        public IEnumerator Walk(float x, float speed = 1f)
        {
            if (animate)
            {
                _animator.Play("Walk");
            }
            
            _sr.flipX = x < 0;
            var destination = transform.position + new Vector3(x, 0, 0);
            while (Util.SqrDistance(transform.position, destination) > .5)
            {
                _rb.velocity = new Vector2(7 * Math.Sign(x) * speed, 0);
                yield return null;
            }

            _rb.velocity = Vector2.zero;
            Debug.Log("STANDING AGAIN");
            if(animate)
                _animator.Play("Stand");
        }
    }
}