using System;
using System.Collections;
using UnityEngine.EventSystems;

namespace Program
{
    public class ChildClickableHandler: ClickableHandler
    {
        private GroupedClickable _parent;

        protected override void Start()
        {
            base.Start();
            _parent = transform.parent.GetComponent<GroupedClickable>();
        }

        public override void OnPointerClick(ClickData e)
        {
            _parent.OnPointerClick(e);
        }

        public override IEnumerator OnMouseEnter()
        {
            yield return _parent.OnMouseEnter();
        }

        public override void OnMouseExit()
        {
            _parent.OnMouseExit();
        }
    }
}