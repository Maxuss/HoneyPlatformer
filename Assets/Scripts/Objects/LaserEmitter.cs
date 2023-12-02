using System;
using System.Collections.Generic;
using System.Linq;
using Program.Action;
using UnityEngine;
using UnityEngine.Serialization;

namespace Objects
{
    [RequireComponent(typeof(AudioSource))]
    public class LaserEmitter: MonoBehaviour
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
        private AudioClip laserLoop;

        private AudioSource _as;
        [SerializeField]
        private bool isActive = false;

        private ParticleSystem[] _particles;
        
        public EmitterColor EmitterType;

        private static readonly int LaserColorFrom = Shader.PropertyToID("_LaserColorFrom");
        private static readonly int LaserColorTo = Shader.PropertyToID("_LaserColorTo");
        
        #region Behaviour

        public void Start()
        {
            _as = GetComponent<AudioSource>();
            _as.clip = laserLoop;
            _as.volume = 0.1f;
            _as.loop = true;
            
            laserLine.material.SetColor(LaserColorFrom, laserColorA);
            laserLine.material.SetColor(LaserColorTo, laserColorB);

            LaserManager.OnMirrorChanged += RecalculateReflections;

            var startParticles = startVfx.GetComponentsInChildren<ParticleSystem>();
            var endParticles = endVfx.GetComponentsInChildren<ParticleSystem>();

            _particles = startParticles.Concat(endParticles).ToArray();

            if (isActive) return;
            
            laserLine.gameObject.SetActive(false);
            _as.Stop();
            foreach (var ps in _particles)
            {
                ps.Stop();
            }
        }

        private void Awake()
        {
            RecalculateReflections();
        }

        private void Update()
        {
            // TODO: remove me, used for testing
            RecalculateReflections();
        }

        [ContextMenu("Toggle Laser")]
        public void Toggle()
        {
            isActive = !isActive;
            if (!isActive)
            {
                laserLine.gameObject.SetActive(false);
                _as.Stop();
                _as.PlayOneShot(laserDeactivate);
                foreach (var ps in _particles)
                {
                    ps.Stop();
                }
            }
            else
            {
                laserLine.gameObject.SetActive(true);
                _as.PlayOneShot(laserActivate);
                _as.Play();
                RecalculateReflections();
                foreach (var ps in _particles)
                {
                    ps.Play();
                }
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
            var direction = transform.right;

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
                        hit.collider.GetComponent<LaserConsumer>().ReceiveLaser(EmitterType);
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

            laserLine.positionCount = verts.Count;
            endVfx.transform.position = (Vector2) verts.Last();
            var secondToLast = verts.Count - 2 < 0 ? transform.position : verts[^2];
            endVfx.transform.rotation = Quaternion.LookRotation(secondToLast - endVfx.transform.position);
            laserLine.SetPositions(verts.ToArray());
        }
        
        #endregion
        
        #region Programming

        public static IAction ToggleLaserAction => new DelegatedAction("Toggle laser", (obj) =>
        {
            obj.wiredObject.GetComponent<LaserEmitter>()?.Toggle();
        });
        
        #endregion
        
        public enum EmitterColor
        {
            Blue,
            Yellow,
            Red,
            Purple
        }
    }
}