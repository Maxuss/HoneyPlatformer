using System.Collections;
using Controller;
using UnityEngine;

namespace Program
{
    public class GroupedClickable: ClickableHandler
    {
        private Renderer[] _childRenderers;

        protected override void Start()
        {
            base.Start();
            _childRenderers = GetComponentsInChildren<Renderer>();
        }

        public override IEnumerator OnMouseEnter()
        {
            if (!CameraController.Instance.VisualEditing.Enabled)
                yield break;
            
            var amount = 0f;

            foreach (var rend in _childRenderers)
            {
                rend.material.SetFloat(ShouldHighlight, 1f);
            }
            while (amount < 1f)
            {
                amount += 1.4f * Time.deltaTime;
                foreach (var rend in _childRenderers)
                {
                    rend.material.SetFloat(HighlightAmount, Mathf.Min(amount, 1f));
                }
                yield return null;
            }
            foreach (var rend in _childRenderers)
            {
                rend.material.SetFloat(HighlightAmount, 1f);
            }
        }

        public  override void OnMouseExit()
        {
            foreach (var rend in _childRenderers)
            {
                rend.material.SetFloat(ShouldHighlight, 0f);
                rend.material.SetFloat(HighlightAmount, 0f);
            }
        }
    }
}