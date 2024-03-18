using System;
using System.Collections.Generic;
using System.Linq;
using Controller;
using Level;
using Program;
using Program.Channel;
using UnityEngine;
using Utils;
using UnityEngine.Serialization;

namespace Objects
{
    public class LaserEmitter: MonoBehaviour, IActionContainer, IChannelReceiver
    {
        [SerializeField]
        private LineRenderer laserLine;
        [SerializeField]
        private Transform laserPos;
        [SerializeField]
        private GameObject startVfx;
        [SerializeField]
        private GameObject endVfx;
        [SerializeField]
        [ColorUsage(false, true)]
        private Color laserColorA;
        [SerializeField]
        [ColorUsage(false, true)]
        private Color laserColorB;
        [SerializeField]
        private LayerMask collisionMask;

        [SerializeField]
        private AudioClip laserActivate;
        [SerializeField]
        private AudioClip laserDeactivate;

        [SerializeField]
        private bool isActive = false;

        private LaserConfig _laserConfig;
        private bool _stateInner;
        public LaserReceiver ConnectedRx { get; private set; }
        private ParticleSystem[] _particles;
        
        public EmitterColor EmitterType;

        private static readonly int LaserColorFrom = Shader.PropertyToID("_LaserColorFrom");
        private static readonly int LaserColorTo = Shader.PropertyToID("_LaserColorTo");
        
        #region Behaviour

        public void Start()
        {
            laserLine.material.SetColor(LaserColorFrom, laserColorA);
            laserLine.material.SetColor(LaserColorTo, laserColorB);

            LaserManager.OnMirrorChanged += RecalculateReflections;

            var startParticles = startVfx.GetComponentsInChildren<ParticleSystem>();
            var endParticles = endVfx.GetComponentsInChildren<ParticleSystem>();

            _particles = startParticles.Concat(endParticles).ToArray();

            laserLine.gameObject.SetActive(isActive);
            if (isActive)
            {
                StartCoroutine(Util.Delay(() =>
                {
                    LaserManager.Instance.ActivateLaser();
                }, 0.5f));
                return;
            }
            
            foreach (var ps in _particles)
            {
                ps.Stop();
            }
            
            RecalculateState();
        }
        private void Update()
        {
            // TODO: replace me with mirror handling
            // (ill remove it when mirrors are done)
            RecalculateReflections();
        }

        [ContextMenu("Toggle Laser")]
        public void Toggle()
        {
            isActive = !isActive;
            if (!isActive)
            {
                laserLine.gameObject.SetActive(false);
                SfxManager.Instance.Play(laserDeactivate, 0.1f);
                foreach (var ps in _particles)
                {
                    ps.Stop();
                }
                LaserManager.Instance.DeactivateLaser();
            }
            else
            {
                laserLine.gameObject.SetActive(true);
                SfxManager.Instance.Play(laserActivate, 0.1f);
                RecalculateReflections();
                foreach (var ps in _particles)
                {
                    ps.Play();
                }
                LaserManager.Instance.ActivateLaser();
            }
        }

        private void RecalculateReflections()
        {
            if (!isActive)
                return;
            
            var currentLength = 18f;
            var currentBounces = 6;
            RaycastHit2D hit;
            var verts = new List<Vector3>();
            verts.Add(laserPos.position);
            var currentPoint = transform.position;
            var direction = transform.up;

            var hitReceiver = false;
            LaserReceiver tmpRx = null;

            while(currentLength > 0 && currentBounces >= 0){
                hit = Physics2D.Raycast(currentPoint, direction, currentLength, collisionMask);
                if (hit)
                {
                    verts.Add(hit.point);
                    currentLength -= Vector3.Distance(currentPoint, hit.point);
                    currentPoint = hit.point;
                    if (hit.collider.CompareTag("Mirror"))
                    {
                        direction = Vector3.Reflect(direction, hit.normal);
                        currentBounces--;
                    }
                    else if (hit.collider.CompareTag("LaserConsumer"))
                    {
                        var rx = hit.collider.GetComponent<LaserReceiver>();
                        if (rx != ConnectedRx)
                        {
                            rx.ReceiveLaser(EmitterType);
                            hitReceiver = true;
                            tmpRx = rx;
                        }

                        currentLength = 0;
                    }
                    else
                    {
                        currentLength = 0;
                    }
                }
                else
                {
                    verts.Add(currentPoint + currentLength * direction);
                    currentLength = 0;
                }
            }

            if (hitReceiver && ConnectedRx != null)
                ConnectedRx.Detach();
            
            if(hitReceiver)
                ConnectedRx = tmpRx!;

            laserLine.positionCount = verts.Count;
            endVfx.transform.position = (Vector2) verts.Last();
            var secondToLast = verts.Count - 2 < 0 ? transform.position : verts[^2];
            endVfx.transform.rotation = Quaternion.LookRotation(secondToLast - endVfx.transform.position);
            laserLine.SetPositions(verts.ToArray());
        }
        
        #endregion
        
        public enum EmitterColor
        {
            Blue,
            Yellow,
            Red,
            Purple
        }

        public string Name => $"Источник Лазера ({EmitterType switch { EmitterColor.Blue => "Синий", EmitterColor.Yellow => "Желтый", EmitterColor.Red => "Красный", EmitterColor.Purple => "Фиолетовый" }})";
        public string Description => "Создает лазерный луч своего цвета в зависимости от настроек.";

        public ActionInfo[] SupportedActions { get; } = {
            new()
            {
                ActionName = "Постоянный луч",
                ActionDescription = "Выводит постоянный луч когда получает сигнал 1 до тех пор пока не поступает сигнал 0."
            },
            new()
            {
                ActionName = "Импульс",
                ActionDescription = "При получении сигнала 1 выводит луч на 1 секунду, затем выключает лазер."
            }
        };

        public ProgrammableType Type { get; } = ProgrammableType.Emitter;
        public ActionData SelectedAction { get; set; }
        public void Begin(ActionData action)
        {
            _laserConfig = (LaserConfig) Enum.ToObject(typeof(LaserConfig), action.ActionIndex);
            RecalculateState();
        }

        private void RecalculateState()
        {
            switch (_laserConfig)
            {
                case LaserConfig.ConstantRay when isActive:
                    laserLine.gameObject.SetActive(true);
                    SfxManager.Instance.Play(laserActivate, 0.1f);
                    RecalculateReflections();
                    foreach (var ps in _particles)
                    {
                        ps.Play();
                    }
                    if (!laserLine.TryGetComponent<MeshCollider>(out _))
                    {
                        var col = laserLine.gameObject.AddComponent<MeshCollider>();
                        var mesh = new Mesh();
                        
                        laserLine.BakeMesh(mesh, CameraController.Instance.gameObject.GetComponent<Camera>(), true);
                        col.sharedMesh = mesh;
                    }
                    LaserManager.Instance.ActivateLaser();
                    break;
                case LaserConfig.ConstantRay or LaserConfig.Impulse when !isActive:
                    if (laserLine == null)
                        return;
                    laserLine.gameObject.SetActive(false);
                    SfxManager.Instance.Play(laserDeactivate, 0.1f);
                    foreach (var ps in _particles)
                    {
                        ps.Stop();
                    }
                    LaserManager.Instance.DeactivateLaser();
                    if(ConnectedRx != null)
                        ConnectedRx.Detach();
                    ConnectedRx = null;
                    break;
                case LaserConfig.Impulse when isActive:
                    laserLine.gameObject.SetActive(true);
                    SfxManager.Instance.Play(laserActivate, 0.1f);
                    RecalculateReflections();
                    foreach (var ps in _particles)
                    {
                        ps.Play();
                    }
                    if (!laserLine.TryGetComponent<MeshCollider>(out _))
                    {
                        var col = laserLine.gameObject.AddComponent<MeshCollider>();
                        var mesh = new Mesh();
                        
                        laserLine.BakeMesh(mesh, CameraController.Instance.gameObject.GetComponent<Camera>(), true);
                        col.sharedMesh = mesh;
                    }
                    LaserManager.Instance.ActivateLaser();
                    StartCoroutine(Util.Delay(() =>
                    {
                        isActive = false;
                        RecalculateState();
                    }, 1));
                    break;
            }
        }

        public void ReceiveBool(Transform source, bool b)
        {
            isActive = b;
            RecalculateState();
        }

        public void ReceiveFloat(Transform source, float v)
        {
            // doing nothing with the float for now
        }
    }

    public enum LaserConfig
    {
        ConstantRay,
        Impulse
    }
}