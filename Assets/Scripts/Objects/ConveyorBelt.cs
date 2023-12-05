using System;
using UnityEngine;
using Utils;

namespace Objects
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class ConveyorBelt: MonoBehaviour
    {
        private bool _enabled = true;
        private Rigidbody2D _rb;

        private void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            if (!_enabled)
                return;
            
            Vector3 oldPos = _rb.position;
            _rb.position += transform.right.XY() * (2f * Time.fixedDeltaTime);
            _rb.MovePosition(oldPos);
        }
    }
}