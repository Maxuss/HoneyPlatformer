using System;
using UnityEngine;
using UnityEngine.Serialization;
using Utils;

namespace Objects
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class ConveyorBelt: MonoBehaviour
    {
        private bool _enabled = false;
        private Rigidbody2D _rb;
        private Material _mat;
        private static readonly int ScrollingSpeed = Shader.PropertyToID("_ScrollingSpeed");

        [SerializeField] private float scrollSpeed = 1f;

        public bool Reversed
        {
            get => scrollSpeed < 0;
            set
            {
                scrollSpeed *= value ? -1 : 1;
                _mat.SetFloat(ScrollingSpeed, scrollSpeed);
            }
        }

        public float ScrollSpeed
        {
            get => Mathf.Abs(scrollSpeed);
            set => scrollSpeed = value * Mathf.Sign(scrollSpeed);
        }
        
        private void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
            _mat = GetComponent<SpriteRenderer>().material;
            
            _mat.SetFloat(ScrollingSpeed, 0f);
        }

        public void Toggle()
        {
            if (_enabled)
            {
                _mat.SetFloat(ScrollingSpeed, 0f);
                _enabled = false;
            }
            else
            {
                _mat.SetFloat(ScrollingSpeed, scrollSpeed);
                _enabled = true;
            }
        }

        private void FixedUpdate()
        {
            if (!_enabled)
                return;
            
            Vector3 oldPos = _rb.position;
            _rb.position += transform.right.XY() * (scrollSpeed * Time.fixedDeltaTime);
            _rb.MovePosition(oldPos);
        }
    }
}