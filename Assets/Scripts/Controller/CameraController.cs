using System;
using Level;
using UnityEngine;
using UnityEngine.U2D;
using Utils;

namespace Controller
{
    [RequireComponent(typeof(Camera))]
    public class CameraController: MonoBehaviour
    {
        [SerializeField]
        private Transform player;
        [SerializeField]
        private float smoothingModifier = 3.5f;

        private Camera _camera;

        private void Start()
        {
            _camera = GetComponent<Camera>();
        }

        private void LateUpdate()
        {
            var targetPos = player.position;
            var currentPos = transform.position;
            if (currentPos == targetPos)
                return;

            // smoothening movement
            var lerpedMovement = Vector2.Lerp(currentPos.XY(), targetPos.XY(), smoothingModifier * Time.deltaTime);
            
            // checking bounds to not move camera on X or
            // Y axis if we are at the bounds of a level
            var (minY, maxY) = VerticalMapBoundaries();
            var (minX, maxX) = HorizontalMapBoundaries();
            lerpedMovement.x = Mathf.Clamp(lerpedMovement.x, minX, maxX);
            lerpedMovement.y = Mathf.Clamp(lerpedMovement.y, minY, maxY);
            
            transform.position = lerpedMovement.ToVec3(currentPos.z);
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
    }
}