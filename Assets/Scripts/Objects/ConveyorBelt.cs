using UnityEngine;
using Utils;

namespace Objects
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class ConveyorBelt: MonoBehaviour
    {
        internal bool Enabled;
        private Rigidbody2D _rb;
        private Material _mat;
        internal bool IsCounterClockwise;
        private static readonly int ScrollingSpeed = Shader.PropertyToID("_ScrollingSpeed");

        [SerializeField] private float scrollSpeed = 1f;
        
        public float ScrollSpeed
        {
            get => Mathf.Abs(scrollSpeed);
            set
            {
                scrollSpeed = value;
                _mat.SetFloat(ScrollingSpeed, scrollSpeed * (IsCounterClockwise ? -1f : 1f) / 4f);
            }
        }

        private void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
            _mat = GetComponent<SpriteRenderer>().material;
            
            _mat.SetFloat(ScrollingSpeed, 0f);
        }

        public void Toggle(bool v)
        {
            Enabled = v;
            if (Enabled)
            {
                var speed = scrollSpeed * (IsCounterClockwise ? -1f : 1f) / 4f;
                _mat.SetFloat(ScrollingSpeed, speed);
            }
            else
            {
                _mat.SetFloat(ScrollingSpeed, 0f);
            }
        }

        private void FixedUpdate()
        {
            if (!Enabled)
                return;

            var speed = scrollSpeed * (IsCounterClockwise ? -1f : 1f);
            Vector3 oldPos = _rb.position;
            _rb.position += transform.right.XY() * (speed * Time.fixedDeltaTime);
            _rb.MovePosition(oldPos);
        }
    }
}