using System;
using System.Collections.Generic;
using UnityEngine;

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
        private List<ParticleSystem> emitParticles;
        [SerializeField]
        private List<ParticleSystem> collideParticles;
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
        private bool _isActive;
        
        public EmitterColor EmitterType;

        private static readonly int LaserColorFrom = Shader.PropertyToID("_LaserColorFrom");
        private static readonly int LaserColorTo = Shader.PropertyToID("_LaserColorTo");

        public void Start()
        {
            _as = GetComponent<AudioSource>();
            _as.clip = laserLoop;
            _as.volume = 0.1f;
            _as.loop = true;
            
            laserLine.material.SetColor(LaserColorFrom, laserColorA);
            laserLine.material.SetColor(LaserColorTo, laserColorB);

            LaserManager.OnMirrorChanged += RecalculateReflections;
        }

        private void Awake()
        {
            RecalculateReflections();
        }

        [ContextMenu("Toggle Laser")]
        public void Toggle()
        {
            _isActive = !_isActive;
            if (!_isActive)
            {
                laserLine.gameObject.SetActive(false);
                _as.Stop();
                _as.PlayOneShot(laserDeactivate);
            }
            else
            {
                laserLine.gameObject.SetActive(true);
                _as.PlayOneShot(laserActivate);
                _as.Play();
                RecalculateReflections();
            }
        }

        private void RecalculateReflections()
        {
            if (!_isActive)
                return;
            
            var currentLength = 10f;
            var currentBounces = 4;
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
            laserLine.SetPositions(verts.ToArray());
        }
        
        public enum EmitterColor
        {
            Blue,
            Yellow,
            Red,
            Purple
        }
    }
}