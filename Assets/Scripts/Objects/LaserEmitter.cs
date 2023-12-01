using System;
using System.Collections.Generic;
using UnityEngine;

namespace Objects
{
    public class LaserEmitter: MonoBehaviour
    {
        [SerializeField]
        private LineRenderer laserLine;
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
        
        public EmitterColor EmitterType;

        private static readonly int LaserColorFrom = Shader.PropertyToID("_LaserColorFrom");
        private static readonly int LaserColorTo = Shader.PropertyToID("_LaserColorTo");

        public void Start()
        {
            laserLine.material.SetColor(LaserColorFrom, laserColorA);
            laserLine.material.SetColor(LaserColorTo, laserColorB);
            laserLine.positionCount = 6;
            laserLine.transform.position = transform.position;
        }

        private List<Vector2> _laserVertices = new();
        private void CastRay(Vector3 pos, Vector3 dir)
        {
            _laserVertices.Add(pos);

            Ray2D ray = new Ray2D(pos, dir);

            var hit = Physics2D.Raycast(pos, dir, 10f, collisionMask);

            if (hit)
            {
                if (!hit.collider.CompareTag("Mirror"))
                {
                    _laserVertices.Add(hit.point);
                    UpdateLaser();
                }
                else
                {
                    var point = hit.point;
                    var newDir = Vector2.Reflect(dir, hit.normal);
                    _laserVertices.Add(hit.point);
                    // CastRay(point, newDir);
                }
            }
            else
            {
                _laserVertices.Add(ray.GetPoint(10f));
            }
        }

        void UpdateLaser()
        {
            int count = 0;
            laserLine.positionCount = _laserVertices.Count;

            foreach (var vertex in _laserVertices)
            {
                laserLine.SetPosition(count, vertex);
                count++;
            }
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