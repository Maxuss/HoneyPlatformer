using System;
using UnityEngine;

namespace Controller
{
    [RequireComponent(typeof(Rigidbody2D), typeof(CapsuleCollider2D))]
    public class PlayerController : MonoBehaviour
    {
        private Rigidbody2D _rb;
        private CapsuleCollider2D _col;
        private bool _grounded;
        private bool _queryStartColliderCached;
        private GatheredInput _input;
        private Vector2 _velocity;
        private bool _earlyJump;
        private bool _consumeJump;
        
        [SerializeField]
        private float moveSpeed = 1f;
        [SerializeField]
        private float acceleration = 0.5f;
        [SerializeField]
        private float fallAcceleration = 0.7f;
        [SerializeField]
        private float fallSpeed = 2f;
        [SerializeField]
        private float jumpForce;
        [SerializeField]
        private float groundDistance = 0.05f;
        [SerializeField]
        private float groundDrag = 1f;
        [SerializeField]
        private float airDrag = 0.8f;
        [SerializeField]
        private LayerMask playerMask;
        
        void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _col = GetComponent<CapsuleCollider2D>();

            _queryStartColliderCached = Physics2D.queriesStartInColliders;
        }

        private void FixedUpdate()
        {
            CollisionCheck();
            
            HandleJump();
            HandleHorizontal();
            HandleGravity();
            
            CommitMovement();
        }

        private void CommitMovement()
        {
            _rb.velocity = _velocity;
        }

        private void HandleHorizontal()
        {
            if (_input.HorizontalMove == 0)
            {
                var drag = _grounded ? groundDrag : airDrag;
                _velocity.x = Mathf.MoveTowards(_velocity.x, 0, drag * Time.fixedDeltaTime);
            }
            else
            {
                _velocity.x = Mathf.MoveTowards(_velocity.x, _input.HorizontalMove * moveSpeed,
                    acceleration * Time.fixedDeltaTime);
            }
        }

        private void HandleGravity()
        {
            if (_grounded && _velocity.y <= 0f)
            {
                _velocity.y = -1.5f;
            }
            else
            {
                _velocity.y = Mathf.MoveTowards(_velocity.y, -fallSpeed, fallAcceleration * Time.fixedDeltaTime);
            }
        }

        private void HandleJump()
        {
            if (!_grounded && !_input.JumpHeld && _rb.velocity.y > 0)
                _earlyJump = true;

            if (!_consumeJump)
                return;

            if (_grounded)
            {
                _velocity.y = jumpForce;
                _earlyJump = false;
            }

            _consumeJump = false;
        }

        private void Jump()
        {
            _earlyJump = false;
            _velocity.y = jumpForce;
        }

        private void CollisionCheck()
        {
            // Physics2D.queriesStartInColliders = false;

            bool groundHit = Physics2D.CapsuleCast(_col.bounds.center, _col.size, _col.direction, 0, Vector2.down, groundDistance, ~playerMask);
            bool ceilingHit = Physics2D.CapsuleCast(_col.bounds.center, _col.size, _col.direction, 0, Vector2.up, groundDistance, ~playerMask);

            if (ceilingHit) _velocity.y = Mathf.Min(0, _velocity.y);

            _grounded = _grounded switch
            {
                false when groundHit => true,
                true when !groundHit => false,
                _ => _grounded
            };

            // Physics2D.queriesStartInColliders = _queryStartColliderCached;
        }
        
        void Update()
        {
            GatherInput();
        }
        
        private void GatherInput()
        {
            _input = new GatheredInput
            {
                JumpDown = Input.GetButtonDown("Jump"),
                JumpHeld = Input.GetButton("Jump"),
                HorizontalMove = Input.GetAxisRaw("Horizontal")
            };

            if (_input.JumpDown)
            {
                _consumeJump = true;
            }
        }

        private struct GatheredInput
        {
            internal bool JumpDown;
            internal bool JumpHeld;
            internal float HorizontalMove;
        }
    }
}
