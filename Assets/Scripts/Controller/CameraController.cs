using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Dialogue;
using Level;
using Nodes;
using Objects;
using Objects.Executors;
using Program;
using Program.UI;
using UnityEngine;
using UnityEngine.Rendering.Universal;
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

        [SerializeField]
        private RectTransform visualEditingNotifier;

        [SerializeField] private RectTransform moreVisualEditingNotifier;

        [SerializeField]
        private Transform globalLight;

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
        public bool DisableFollow { get; set; }

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
            if (Input.GetKeyDown(KeyCode.V) && !NodeManager.Instance.isEditing)
            {
                if(_inProgram)
                    ExitProgramMode();
                else if(!PlayerController.Instance.IsDisabled)
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
            if (DisableFollow)
                return;
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

        public IEnumerator TransitionToPoint(Vector2 towards, float speed = 1f)
        {
            _inTransition = true;
            var velocity = Vector2.zero;
            while (Util.SqrDistance(transform.position, towards) > 0.5f)
            {
                var pos = transform.position;
                transform.position = Vector2.SmoothDamp(pos, towards, ref velocity, 0.4f * speed).ToVec3(pos.z);
                yield return null;
            }

            _inTransition = false;
        }

        [ContextMenu("Enter Program")]
        public void EnterProgramMode()
        {
            if (_transitioningProgram || _inProgram)
                return;
            _inProgram = true;
            StartCoroutine(EnterProgramEffect());
            StartCoroutine(IncreaseBrightness());
            StartCoroutine(Util.Delay(MoveToastInside, 0.2f));
            _visual.Enabled = true;
            PlayerController.Instance.IsDisabled = true;
            
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        [ContextMenu("Exit Program")]
        public void ExitProgramMode()
        {
            if (_transitioningProgram || !_inProgram)
                return;
            _inProgram = false;
            StartCoroutine(ExitProgramEffect());
            StartCoroutine(DecreaseBrightness());
            StartCoroutine(Util.Delay(HideToast, 0.2f));
            _visual.Enabled = false;
            _visual.FinishConnection();
            _visual.ClearLines();
            PlayerController.Instance.IsDisabled = false;
            ProgrammableUIManager.Instance.Close();
            _visual.Editing = false;

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void MoveToastInside()
        {
            visualEditingNotifier.gameObject.SetActive(true);
            visualEditingNotifier.DOAnchorPos(new Vector3(80f, 40f), 1f);
        }

        private void HideToast()
        {
            visualEditingNotifier.DOAnchorPos(new Vector3(-80f, 40f), 1f);
            moreVisualEditingNotifier.DOAnchorPos(new Vector2(-80f, 80f), .5f);
        }

        private IEnumerator ExitProgramEffect()
        {
            _transitioningProgram = true;
            SfxManager.Instance.Play(exitEditModeSound, .7f);
            
            var mats = new List<Material>();
            
            foreach (var programmable in Util.GetAllComponents<IActionContainer>())
            {
                if (programmable is ConveyorGroup group)
                {
                    // grouped objects go here
                    foreach (var belt in group.InnerBelts)
                    {
                        belt.GetComponent<Renderer>().material.SetFloat(OutlineThickness, 0f);
                    }

                    continue;
                }
                var render = (programmable as MonoBehaviour)?.GetComponent<Renderer>();
                mats.Add(render!.material);
            }

            StartCoroutine(DecreaseObjectOutline(mats));
            
            var amount = .9f;
            while (amount > 0f)
            {
                amount -= 0.9f * Time.fixedDeltaTime;
                effectRenderer.material.SetFloat(StartAnimationProgress, Mathf.Max(0f, amount));
                yield return null;
            }
            effectRenderer.material.SetFloat(Enabled, 0f);
            effectRenderer.material.SetFloat(StartAnimationProgress, 0f);

            _transitioningProgram = false;
        }

        private IEnumerator EnterProgramEffect()
        {
            _transitioningProgram = true;
            SfxManager.Instance.Play(enterEditModeSound, .7f);
            effectRenderer.material.SetFloat(Enabled, 1f);
            
            var materials = new List<Material>();
            foreach (var programmable in Util.GetAllComponents<IActionContainer>())
            {
                if (programmable is ConveyorGroup group)
                {
                    // grouped objects go here
                    foreach (var belt in group.InnerBelts)
                    {
                        belt.GetComponent<Renderer>().material.SetFloat(OutlineThickness, 1f);
                    }

                    continue;
                }
                var render = (programmable as MonoBehaviour)?.GetComponent<Renderer>();
                materials.Add(render!.material);
            }

            StartCoroutine(IncreaseObjectsOutline(materials));
            
            var amount = 0f;
            
            while (amount < 1f)
            {
                amount += 0.5f * Time.fixedDeltaTime;
                effectRenderer.material.SetFloat(StartAnimationProgress, amount);
                yield return null;
            }
            effectRenderer.material.SetFloat(StartAnimationProgress, 1f);

            
            _transitioningProgram = false;
        }

        private IEnumerator IncreaseObjectsOutline(List<Material> mats)
        {
            var i = 0f;
            while (i < 1f)
            {
                i += 0.9f * Time.deltaTime;
                mats.ForEach(each => each.SetFloat(OutlineThickness, i));
                yield return null;
            }
            mats.ForEach(each => each.SetFloat(OutlineThickness, 1f));
        }
        
        private IEnumerator DecreaseObjectOutline(List<Material> mats)
        {
            var i = 1f;
            while (i > 0f)
            {
                i -= 0.9f * Time.deltaTime;
                mats.ForEach(each => each.SetFloat(OutlineThickness, i));
                yield return null;
            }
            mats.ForEach(each => each.SetFloat(OutlineThickness, 0f));
        }


        private IEnumerator IncreaseBrightness()
        {
            var amount = .7f;
            var light2D = globalLight.GetComponent<Light2D>();
            while (amount < .85f)
            {
                amount += .25f * Time.deltaTime;
                light2D.intensity = amount;
                yield return null;
            }
            light2D.intensity = .85f;
        }
        
        private IEnumerator DecreaseBrightness()
        {
            var amount = .85f;
            var light2D = globalLight.GetComponent<Light2D>();
            while (amount > .5f)
            {
                amount -= .25f * Time.deltaTime;
                light2D.intensity = amount;
                yield return null;
            }
            light2D.intensity = .7f;
        }
    }
}