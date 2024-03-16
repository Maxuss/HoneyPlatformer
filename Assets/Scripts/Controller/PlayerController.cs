using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Level;
using Program.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Utils;
using Random = UnityEngine.Random;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace Controller
{
    [RequireComponent(
        typeof(Rigidbody2D), typeof(BoxCollider2D))]
    public class PlayerController : MonoBehaviour
    {
        #region Movement
        
        private Rigidbody2D _rb;
        private BoxCollider2D _col;
        private Animator _anim;
        private SpriteRenderer _spriteRenderer;
        
        private bool _grounded;
        private bool _groundedLastFrame;
        private bool _queryStartColliderCached;
        private GatheredInput _input;
        private Vector2 _velocity;
        private bool _earlyJump;
        private bool _consumeJump;
        private bool _coyoteUsable;
        private bool _hasBufferedJump;
        private float _startedJumping;

        private float _time;
        private float _frameLeftGround = float.MinValue;
        private float _lastJumpPressed;
        private float _defaultGravityScale;

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
        
        [Header("Movement")]
        [SerializeField]
        private float moveSpeed = 7f;
        [SerializeField]
        private float acceleration = 40f;
        [SerializeField]
        private float fallAcceleration = 35f;
        [SerializeField]
        private float fallSpeed = 40f;
        [SerializeField]
        private float jumpForce = 13;
        [SerializeField]
        private float groundDistance = 0.05f;
        [SerializeField]
        private float groundDrag = 200f;
        [SerializeField]
        private float airDrag = 150f;
        [SerializeField]
        private LayerMask notTerrainMask;
        
        [Space(20)]
        [SerializeField]
        private List<AudioClip> footstepSounds;
        [SerializeField]
        private AudioClip jumpSound;
        [SerializeField]
        private AudioClip landSound;
        [SerializeField] 
        private float timeBetweenFootsteps = 0.3f;

        [Space(20)] 
        [SerializeField]
        private ParticleSystem jumpParticles;
        [SerializeField]
        private ParticleSystem landParticles;

        private GameObject _currentPlatform;

        private bool _autonomous;

        private static readonly int Speed = Animator.StringToHash("Speed");
        private static readonly int IsJumping = Animator.StringToHash("IsJumping");
        private static readonly int IsPulling = Animator.StringToHash("IsPulling");
        private static readonly int IsPushing = Animator.StringToHash("IsPushing");

        private bool HasBufferedJump => _hasBufferedJump && _time < _lastJumpPressed + .1f;
        private bool CanUseCoyote => _coyoteUsable && !_grounded && _time < _frameLeftGround + .12f;

        public bool IsDisabled { get; set; } = false;
        public bool StillCommitMovement { get; set; } = false;
        public bool InCutscene { get; set; } = false;
        
        public static PlayerController Instance { get; private set; }
        
        public Vector2 Velocity
        {
            get => _velocity;
            set => _velocity = value;
        }

        public IEnumerator AutonomousMove(Vector2 towards)
        {
            _autonomous = true;
            _velocity = Vector2.zero;
            while (Util.SqrDistance(transform.position, towards) > 0.25f)
            {
                _velocity = Vector3.right * 3f;
                _rb.velocity = Vector2.right * 3f;
                yield return null;
            }
            _autonomous = false;
        }
        
        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _col = GetComponent<BoxCollider2D>();
            _anim = GetComponent<Animator>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _defaultGravityScale = _rb.gravityScale;

            _queryStartColliderCached = Physics2D.queriesStartInColliders;
            
            _black = ProgrammableUIManager.Instance.Canvas.Find("Black").GetComponent<Image>();

            Instance = this;
            
            DontDestroyOnLoad(this);
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
        
        private Image _black;
        private IEnumerator RestartLevel(EntranceDoor obj)
        {
            var active = SceneManager.GetActiveScene();
            IsDisabled = true;
            yield return FadeIn();
            transform.position = obj.GetComponent<EntranceDoor>().restartPosition.position;
            var load = SceneManager.LoadSceneAsync(active.buildIndex);
            load.completed += action =>
            {
                StartCoroutine(FadeOut());
                IsDisabled = false;
                var newDoor = SceneManager.GetSceneAt(0).GetRootGameObjects()
                    .First(it => it.CompareTag("EntranceDoor"));
                newDoor.GetComponent<Animator>().Play("LockDoor");
                newDoor.GetComponent<BoxCollider2D>().enabled = true;
                Debug.Log($"{newDoor.GetComponent<BoxCollider2D>().enabled}");
            };
        }


        public IEnumerator FadeIn()
        {
            _black.gameObject.SetActive(true);
            _black.color = new Color(0f, 0f, 0f, 0f);
            var opacity = 0f;
            
            while (opacity < 1f)
            {
                opacity += Time.deltaTime;
                _black.color = new Color(0f, 0f, 0f, opacity);
                yield return null;
            }
            _black.color = new Color(0f, 0f, 0f, 1f);
        }

        public void BlackOut()
        {     
            _black.gameObject.SetActive(true);
            _black.color = new Color(0f, 0f, 0f, 1f);
        }

        public void ShowTBC()
        {
            _black.transform.parent.Find("ToBeContinued").gameObject.SetActive(true);
        }
        
        public IEnumerator FadeOut()
        {
            var opacity = 0f;
            
            while (opacity > 0f)
            {
                opacity -= Time.deltaTime;
                _black.color = new Color(0f, 0f, 0f, opacity);
                yield return null;
            }
            _black.color = new Color(0f, 0f, 0f, 0f);
            _black.gameObject.SetActive(false);
        }

        private void CommitMovement()
        {
            if (IsDisabled && !StillCommitMovement)
                // zero x velocity if we are in UI
                _velocity.x = 0;
            _rb.velocity = _velocity;
            if (_rb.velocity.y < 0)
                _rb.gravityScale = _defaultGravityScale * 2f;
            else
                _rb.gravityScale = _defaultGravityScale;
            _anim.SetFloat(Speed, Mathf.Abs(_velocity.x));
            if (_isDragging)
            {
                var diff = _draggedObject.transform.position - transform.position;
                _spriteRenderer.flipX = diff.x <= 0; // object is to the left of us
            }
            else
            {
                _spriteRenderer.flipX = _spriteRenderer.flipX ? _velocity.x <= 0f : _velocity.x < 0f;
            }
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
                SfxManager.Instance.Play(clip, 0.35f);
            }
        }

        private void HandleDirectionChange()
        {
            // changing grab transform position
            var position = handGrabTransform.position;
            position =
                new Vector3(transform.position.x + (facingDirection == FacingDirection.Left ? -.5f : .5f),
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
            if (_velocity.y > 0 && !_input.JumpHeld && _time - _startedJumping > .1f)
            {
                _velocity.y *= .6f;
            }
            
            if (!_earlyJump && !_grounded && !_input.JumpHeld && _rb.velocity.y > 0)
                _earlyJump = true;

            // cant jump if dragging an object
            if ((!_consumeJump && !HasBufferedJump) || _isDragging)
                return;

            if (_grounded || CanUseCoyote)
            {
                _velocity.y = jumpForce;
                
                _anim.SetBool(IsJumping, true);
                SfxManager.Instance.Play(jumpSound, 0.1f);
                jumpParticles.Play();

                _earlyJump = false;
                _coyoteUsable = false;
                _hasBufferedJump = false;
                _lastJumpPressed = 0;
                _startedJumping = _time;
            }

            _consumeJump = false;
        }

        private void CollisionCheck()
        {
            Physics2D.queriesStartInColliders = false;

            bool groundHit = Physics2D.BoxCast(_col.bounds.center, _col.size, 0, Vector2.down, groundDistance, ~notTerrainMask);
            bool ceilingHit = Physics2D.BoxCast(_col.bounds.center, _col.size, 0, Vector2.up, groundDistance, ~notTerrainMask);

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

            if (_grounded && !_groundedLastFrame)
            {
                _anim.SetBool(IsJumping, false);
                SfxManager.Instance.Play(landSound, 0.1f);
                landParticles.Play();
            }

            _groundedLastFrame = _grounded;
            Physics2D.queriesStartInColliders = _queryStartColliderCached;

            if (!_input.DownPressed || _currentPlatform == null) return;
            
            var platformCollider = _currentPlatform.GetComponent<BoxCollider2D>();

            Physics2D.IgnoreCollision(_col, platformCollider);
            StartCoroutine(Util.Delay(() => Physics2D.IgnoreCollision(_col, platformCollider, false), 1f));
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Platform"))
                _currentPlatform = other.gameObject;
        }

        private void OnCollisionExit2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Platform"))
                _currentPlatform = null;
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
                    _anim.SetBool(IsPulling, false);
                    _anim.SetBool(IsPushing, false);
                    break;
                case true when !_isDragging && _grounded:
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
                        
                        _anim.SetBool(IsPushing, true);
                    }
                    break;
                }
            }
        }
        
        private void Update()
        {
            _time += Time.deltaTime;
            if(!IsDisabled && !_autonomous)
                GatherInput();
            
            if (!IsDisabled && _input.RestartRequested)
            {
                var obj = SceneManager.GetSceneAt(0).GetRootGameObjects().FirstOrDefault(it => it.CompareTag("EntranceDoor"))
                    ?.GetComponent<EntranceDoor>();
                if (obj != null && SceneManager.GetSceneAt(0).buildIndex > 0)
                {
                    StartCoroutine(RestartLevel(obj));
                    _input.RestartRequested = false;
                }
            }
        }
        
        private void GatherInput()
        {
            if (InCutscene)
            {
                _input = new GatheredInput();
                return;
            }
            _input = new GatheredInput
            {
                JumpDown = Input.GetButtonDown("Jump"),
                JumpHeld = Input.GetButton("Jump"),
                HorizontalMove = Input.GetAxisRaw("Horizontal"),
                GrabHeld = Input.GetKey(KeyCode.LeftShift),
                DownPressed = Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow),
                RestartRequested = Input.GetKeyDown(KeyCode.R)
            };

            if (!_input.JumpDown) return;
            
            _consumeJump = true;
            _lastJumpPressed = _time;
        }

        private struct GatheredInput
        {
            internal bool JumpDown;
            internal bool JumpHeld;
            internal float HorizontalMove;
            internal bool GrabHeld;
            internal bool DownPressed;
            internal bool RestartRequested;
        }

        public enum FacingDirection
        {
            Left,
            Right
        }
    }
}
