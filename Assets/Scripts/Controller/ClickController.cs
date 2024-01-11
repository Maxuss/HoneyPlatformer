using System;
using Program;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Controller
{
    public class ClickController : MonoBehaviour
    {
        private Camera _camera;

        private void Awake()
        {
            _camera = Camera.main;
        }

        public void OnClick(InputAction.CallbackContext ctx)
        {
            if (!ctx.started) return;

            Debug.Log("RAYCASTING");
            var rayHit = Physics2D.GetRayIntersection(_camera.ScreenPointToRay(Mouse.current.position.ReadValue()));
            if (!rayHit.collider) return;
            Debug.Log("RAY HIT");
            if (rayHit.collider.TryGetComponent<ClickableHandler>(out var clickable))
            {
            }
        }
    }
}