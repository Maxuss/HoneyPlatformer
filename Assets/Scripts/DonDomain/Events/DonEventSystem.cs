using System;
using Controller;
using UnityEngine;
using Utils;

namespace DonDomain.Events
{
    public class DonEventSystem: MonoBehaviour
    {
        public static DonEventSystem Instance { get; private set; }

        public Vector3 MouseWorldPosition { get; private set; }
        public UpgradeCapsule HoveredCapsule { get; private set; }

        public Transform boundsBottomLeft;
        public Transform boundsTopRight;

        [SerializeField]
        private Transform nodes;
        [SerializeField]
        private CircleCollider2D circleMouse;

        private void Awake()
        {
            Instance = this;
        }

        private Vector3 mousePos = Vector3.zero;
        
        private void LateUpdate()
        {
            PollMousePos();
            
            PollClick();
            
            // PollScroll(); - rly bugged atm
            
            PollHold();
            
            PollUnclick();
        }


        private void PollDragTick(Vector2 oldPos)
        {
            if (!Input.GetMouseButton(0))
                return;

            if (HoveredCapsule != null && !HoveredCapsule.CanDragThrough)
                return;

            var delta = MouseWorldPosition.XY() - oldPos;
            if (delta.sqrMagnitude > 1)
                delta = Vector2.zero;
            nodes.Translate(delta);
            nodes.transform.position = nodes.transform.position.CoerceIn(boundsBottomLeft.position, boundsTopRight.position);
        }

        private void PollScroll()
        {
            var delta = Input.mouseScrollDelta;
            if(delta.sqrMagnitude > 0)
                nodes.localScale = (nodes.localScale + delta.y * 10 * Time.deltaTime * Vector3.one).CoerceIn(new Vector3(0.6f, 0.6f, 0.6f), Vector3.one);
        }

        private void PollClick()
        {
            // TODO: pretty ripple :3
            if(Input.GetMouseButtonDown(0) && HoveredCapsule != null)
                HoveredCapsule.OnClick();
        }

        private void PollUnclick()
        {
            if (Input.GetMouseButtonUp(0) && HoveredCapsule != null)
            {
                HoveredCapsule.OnUnclick();
            }
        }

        private void PollHold()
        {
            if (Input.GetMouseButton(0) && HoveredCapsule != null)
            {
                HoveredCapsule.HoldTick();
            }
        }

        private void PollMousePos()
        {
            if (Input.mousePosition == mousePos)
                return;
            
            mousePos = Input.mousePosition;
            var vPoint = Camera.main!.ScreenToViewportPoint(mousePos);
            var pos = vPoint;
            pos.x *= (1920f / Screen.width);
            pos.y *= (1280f / Screen.height);
            pos = Camera.main!.ViewportToWorldPoint(pos);
            // pos.x *= 1920f / Screen.width;
            // pos.y *= 1280f / Screen.height;
            var oldPos = MouseWorldPosition;
            MouseWorldPosition = pos;
            
            // unrelated kinda
            PollDragTick(oldPos);
            
            var hit = Physics2D.OverlapPoint(pos);
            if (hit != null && hit.TryGetComponent<UpgradeCapsule>(out var capsule))
            {
                if (HoveredCapsule == capsule)
                    return;
                HoveredCapsule = capsule;
                capsule.OnHover();
            } else if (hit == null && HoveredCapsule != null)
            {
                HoveredCapsule.OnUnhover();
                HoveredCapsule = null;
            }
        }
    }
}