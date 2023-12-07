using System;
using System.Collections.Generic;
using System.Linq;
using Level;
using UnityEngine;
using Utils;
using UnityEngine.Serialization;

namespace Objects
{
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
        private bool isActive = false;

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
            var currentPoint = transform.position + (-transform.right * 0.5f + transform.up * 0.5f);
            var direction = transform.up;

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
            // TODO: fix normals?
            // TODO: do it!!
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
    }
}