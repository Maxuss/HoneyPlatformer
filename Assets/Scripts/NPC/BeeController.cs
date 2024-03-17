using System;
using Objects;
using UnityEngine;

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

        public bool Active => !_escaping;

        private void Start()
        {
            _center = transform.position;
            _sr = GetComponent<SpriteRenderer>();
        }

        public void FixedUpdate()
        {
            if (_escaping)
            {
                _escapeCycle += 2 * Time.fixedDeltaTime;
                transform.position += 0.025f * Mathf.Max(_escapeCycle, 8f) * new Vector3(0, 1f);
                return;
            }
            _rotationCycle += rotationSpeed * Time.fixedDeltaTime;

            var y = Mathf.Sin(Mathf.Deg2Rad * (_rotationCycle % 360f));
            var x = Mathf.Cos(Mathf.Deg2Rad * (_rotationCycle % 360f));
            var tf = transform;
            tf.position = _center + new Vector3(x * radius, y * radius, 0);
            var euler = Quaternion.LookRotation(tf.position - _center).eulerAngles;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, euler.x));
        }
        
        public void Escape()
        {
            _escaping = true;
            ravagedWires.BeeLeaving();
        }
    }
}