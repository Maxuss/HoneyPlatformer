using System;
using UnityEngine;

namespace Controller
{
    public class VisualEditingMode: MonoBehaviour
    {
        [SerializeField]
        private float smootheningModifier = 0.1f;

        [SerializeField] private Transform renderLineContainer;

        public bool Enabled { get; set; }
        public bool Editing { get; set; }

        private Camera _camera;
        private Vector3 _velocity;

        // TODO: wire rendering
        private void Start()
        {
            _camera = GetComponent<Camera>();
        }
        
        public void FixedUpdate()
        {
            if (!Enabled || Editing)
                return;

            var horizontal = Vector2.right * Input.GetAxisRaw("Horizontal");
            var vertical = Vector2.up * Input.GetAxisRaw("Vertical");
            var newPos = transform.position + (Vector3) (horizontal + vertical);
            transform.position = 
                Vector3.SmoothDamp(transform.position, newPos, ref _velocity, smootheningModifier, Mathf.Infinity);
        }
    }
}