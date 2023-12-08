using System;
using System.Collections;
using Level;
using Program;
using Program.UI;
using UnityEngine;
using UnityEngine.U2D;
using Utils;

namespace Controller
{
    [RequireComponent(typeof(Camera), typeof(VisualEditingMode))]
    public class CameraController: MonoBehaviour
    {
        [SerializeField]
        private Transform player;
        [SerializeField]
        private float smoothingModifier = 3.5f;
        [SerializeField]
        private Renderer effectRenderer;

        [SerializeField]
        private AudioClip enterEditModeSound;
        [SerializeField]
        private AudioClip exitEditModeSound;

        private Camera _camera;
        private VisualEditingMode _visual;

        private bool _inTransition;
        private static readonly int Enabled = Shader.PropertyToID("_Enabled");
        private static readonly int StartAnimationProgress = Shader.PropertyToID("_StartAnimationProgress");

        private bool _inProgram;
        private bool _transitioningProgram;
        private static readonly int OutlineThickness = Shader.PropertyToID("_OutlineThickness");

        public static CameraController Instance { get; private set; }
        public VisualEditingMode VisualEditing => _visual;

        private void Start()
        {
            _camera = GetComponent<Camera>();
            _visual = GetComponent<VisualEditingMode>();
            Instance = this;
        }

        private void Awake()
        {
            DontDestroyOnLoad(this);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.V))
            {
                if(_inProgram)
                    ExitProgramMode();
                else
                    EnterProgramMode();
            }
        }

        private void LateUpdate()
        {
            if(!_inTransition && !_inProgram)
                FollowPlayer();
            else if (_inProgram)
            {
                var pos = (Vector2) transform.position;
                ClampIntoBounds(ref pos);
                transform.position = pos.ToVec3(transform.position.z);
            }
        }

        private void FollowPlayer()
        {
            var targetPos = player.position;
            var currentPos = transform.position;
            if (currentPos == targetPos)
                return;

            // smoothening movement
            var lerpedMovement = Vector2.Lerp(currentPos.XY(), targetPos.XY(), smoothingModifier * Time.deltaTime);
            
            ClampIntoBounds(ref lerpedMovement);
            
            transform.position = lerpedMovement.ToVec3(currentPos.z);
        }

        private void ClampIntoBounds(ref Vector2 movement)
        {
            // checking bounds to not move camera on X or
            // Y axis if we are at the bounds of a level
            var (minY, maxY) = VerticalMapBoundaries();
            var (minX, maxX) = HorizontalMapBoundaries();
            movement.x = Mathf.Clamp(movement.x, minX, maxX);
            movement.y = Mathf.Clamp(movement.y, minY, maxY);

        }
        
        private (float, float) VerticalMapBoundaries()
        {
            var verticalExtent = _camera.orthographicSize;
            
            return (
                LevelManager.Instance.MapBounds.min.y + verticalExtent,
                LevelManager.Instance.MapBounds.max.y - verticalExtent
                );
        }

        private (float, float) HorizontalMapBoundaries()
        {
            var horizontalExtent = _camera.aspect * _camera.orthographicSize;

            return (
                LevelManager.Instance.MapBounds.min.x + horizontalExtent,
                LevelManager.Instance.MapBounds.max.x - horizontalExtent
                );
        }

        public IEnumerator TransitionToPoint(Vector2 towards)
        {
            _inTransition = true;
            var velocity = Vector2.zero;
            while (Util.SqrDistance(transform.position, towards) > 0.5f)
            {
                var pos = transform.position;
                transform.position = Vector2.SmoothDamp(pos, towards, ref velocity, 0.4f).ToVec3(pos.z);
                yield return null;
            }

            _inTransition = false;
        }

        [ContextMenu("Enter Program")]
        public void EnterProgramMode()
        {
            if (_transitioningProgram)
                return;
            _inProgram = true;
            StartCoroutine(EnterProgramEffect());
            _visual.Enabled = true;
            PlayerController.Instance.IsDisabled = true;
            
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        [ContextMenu("Exit Program")]
        public void ExitProgramMode()
        {
            if (_transitioningProgram)
                return;
            _inProgram = false;
            StartCoroutine(ExitProgramEffect());
            _visual.Enabled = false;
            PlayerController.Instance.IsDisabled = false;
            ProgrammableUIManager.Instance.Close();
            _visual.Editing = false;

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        public IEnumerator ExitProgramEffect()
        {
            _transitioningProgram = true;
            SfxManager.Instance.Play(exitEditModeSound, .7f);
            
            foreach (var programmable in Util.GetAllComponents<IActionContainer>())
            {
                var render = (programmable as MonoBehaviour)?.GetComponent<Renderer>();
                render!.material.SetFloat(OutlineThickness, 0f);
            }
            
            var amount = .9f;
            while (amount > 0f)
            {
                amount -= 0.4f * Time.fixedDeltaTime;
                effectRenderer.material.SetFloat(StartAnimationProgress, Mathf.Max(0f, amount));
                yield return null;
            }
            effectRenderer.material.SetFloat(Enabled, 0f);
            effectRenderer.material.SetFloat(StartAnimationProgress, 0f);
            
            foreach (var programmable in Util.GetAllComponents<IActionContainer>())
            {
                var render = (programmable as MonoBehaviour)?.GetComponent<Renderer>();
                render!.material.SetFloat(OutlineThickness, 0f);
            }

            _transitioningProgram = false;
        }

        public IEnumerator EnterProgramEffect()
        {
            _transitioningProgram = true;
            SfxManager.Instance.Play(enterEditModeSound, .7f);
            effectRenderer.material.SetFloat(Enabled, 1f);
            
            var amount = 0f;

            foreach (var programmable in Util.GetAllComponents<IActionContainer>())
            {
                var render = (programmable as MonoBehaviour)?.GetComponent<Renderer>();
                render!.material.SetFloat(OutlineThickness, 1f);
            }

            while (amount < 1f)
            {
                amount += 0.3f * Time.fixedDeltaTime;
                effectRenderer.material.SetFloat(StartAnimationProgress, amount);
                yield return null;
            }
            effectRenderer.material.SetFloat(StartAnimationProgress, 1f);
            _transitioningProgram = false;
        }
    }
}