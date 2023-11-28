using System;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace Controller
{
    [RequireComponent(typeof(Rigidbody2D), typeof(CapsuleCollider2D), typeof(AudioSource))]
    public class PlayerController : MonoBehaviour
    {
        #region Movement
        
        private Rigidbody2D _rb;
        private CapsuleCollider2D _col;
        private bool _grounded;
        private bool _queryStartColliderCached;
        private GatheredInput _input;
        private Vector2 _velocity;
        private bool _earlyJump;
        private bool _consumeJump;
        private bool _coyoteUsable;
        private bool _hasBufferedJump;

        private float _time;
        private float _frameLeftGround = float.MinValue;
        private float _lastJumpPressed;

        public FacingDirection facingDirection = FacingDirection.Left;
        private float _lastFootstep;
        
        #endregion
        
        #region Dragging
        private FixedJoint2D _draggedObject;
        private bool _isDragging;

        [Header("Grabbing objects")]
        [SerializeField]
        private Transform handGrabTransform;
        [SerializeField]
        private LayerMask grabLayer;
        
        #endregion

        private AudioSource _as;
        
        [Header("Movement")]
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
        
        [Space(20)]
        [SerializeField]
        private List<AudioClip> footstepSounds;
        [SerializeField]
        private AudioClip jumpSound;

        [SerializeField] 
        private float timeBetweenFootsteps = 0.5f;
       
        private bool HasBufferedJump => _hasBufferedJump && _time < _lastJumpPressed + .1f;
        private bool CanUseCoyote => _coyoteUsable && !_grounded && _time < _frameLeftGround + .15f;

        public bool IsDisabled { get; set; } = false;
        
        public static PlayerController Instance { get; private set; }
        
        void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _col = GetComponent<CapsuleCollider2D>();
            _as = GetComponent<AudioSource>();

            _queryStartColliderCached = Physics2D.queriesStartInColliders;

            Instance = this;
        }

        private void FixedUpdate()
        {
            CollisionCheck();
            
            HandleJump();
            HandleHorizontal();
            HandleGrab();
            HandleGravity();
            
            CommitMovement();
        }

        private void CommitMovement()
        {
            if (IsDisabled)
                // zero x velocity if we are in UI
                _velocity.x = 0;
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
                var oldDir = facingDirection;
                facingDirection = _input.HorizontalMove > 0 ? FacingDirection.Right : FacingDirection.Left;
                if (oldDir != facingDirection)
                    HandleDirectionChange();
                _velocity.x = Mathf.MoveTowards(_velocity.x, _input.HorizontalMove * moveSpeed * (_isDragging ? 0.8f : 1f),
                    acceleration * Time.fixedDeltaTime);
                
                if (!_grounded || Time.time - _lastFootstep < timeBetweenFootsteps) return;
                
                // playing footstep sounds
                _lastFootstep = Time.time;
                var clip = footstepSounds[Random.Range(0, footstepSounds.Count)];
                _as.PlayOneShot(clip, 0.2f);
            }
        }

        private void HandleDirectionChange()
        {
            // changing grab transform position
            var position = handGrabTransform.position;
            position =
                new Vector3(transform.position.x + (facingDirection == FacingDirection.Left ? -.8f : .8f),
                    position.y, position.z);
            handGrabTransform.position = position;
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
            if (!_earlyJump && !_grounded && !_input.JumpHeld && _rb.velocity.y > 0)
                _earlyJump = true;

            // cant jump if dragging an object
            if ((!_consumeJump && !HasBufferedJump) || _isDragging)
                return;

            if (_grounded || CanUseCoyote)
            {
                _velocity.y = jumpForce;
                _as.PlayOneShot(jumpSound);
                _earlyJump = false;
                _coyoteUsable = false;
                _hasBufferedJump = false;
                _lastJumpPressed = 0;
            }

            _consumeJump = false;
        }

        private void CollisionCheck()
        {
            Physics2D.queriesStartInColliders = false;

            bool groundHit = Physics2D.CapsuleCast(_col.bounds.center, _col.size, _col.direction, 0, Vector2.down, groundDistance, ~playerMask);
            bool ceilingHit = Physics2D.CapsuleCast(_col.bounds.center, _col.size, _col.direction, 0, Vector2.up, groundDistance, ~playerMask);

            if (ceilingHit) _velocity.y = Mathf.Min(0, _velocity.y);

            switch (_grounded)
            {
                case false when groundHit:
                    _grounded = true;
                    _coyoteUsable = true;
                    _earlyJump = false;
                    _hasBufferedJump = true;
                    break;
                case true when !groundHit:
                    _grounded = false;
                    _frameLeftGround = _time;
                    break;
            }
            
            Physics2D.queriesStartInColliders = _queryStartColliderCached;
        }

        private void HandleGrab()
        {
            switch (_input.GrabHeld)
            {
                case false when _isDragging:
                    _draggedObject.connectedBody = null;
                    _draggedObject.enabled = false;
                    _draggedObject = null;
                    _isDragging = false;
                    break;
                case true when !_isDragging:
                {
                    var hit = Physics2D.Raycast(
                        handGrabTransform.position,
                        facingDirection == FacingDirection.Left ? Vector3.left : Vector3.right,
                        0.1f, grabLayer
                    );
                    
                    if (hit.collider != null)
                    {
                        _draggedObject = hit.collider.gameObject.GetComponent<FixedJoint2D>();
                        _draggedObject.connectedBody = _rb;
                        _draggedObject.enabled = true;
                        _isDragging = true;
                    }
                    break;
                }
            }
        }
        
        void Update()
        {
            _time += Time.deltaTime;
            if(!IsDisabled)
                GatherInput();
        }
        
        private void GatherInput()
        {
            _input = new GatheredInput
            {
                JumpDown = Input.GetButtonDown("Jump"),
                JumpHeld = Input.GetButton("Jump"),
                HorizontalMove = Input.GetAxisRaw("Horizontal"),
                GrabHeld = Input.GetKey(KeyCode.LeftShift)
            };

            if (_input.JumpDown)
            {
                _consumeJump = true;
                _lastJumpPressed = _time;
            }
        }

        private struct GatheredInput
        {
            internal bool JumpDown;
            internal bool JumpHeld;
            internal float HorizontalMove;
            internal bool GrabHeld;
        }

        public enum FacingDirection
        {
            Left,
            Right
        }
    }
}
