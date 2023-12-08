using System;
using System.Collections;
using Controller;
using Program.Channel;
using Program.UI;
using UnityEngine;

namespace Program
{
    public class HighlightHandler: MonoBehaviour
    {
        private Renderer _renderer;
        private IActionContainer _actionContainer;
        private static readonly int ShouldHighlight = Shader.PropertyToID("_ShouldHighlight");
        private static readonly int HighlightAmount = Shader.PropertyToID("_HighlightAmount");

        private void Start()
        {
            _renderer = GetComponent<Renderer>();
            _actionContainer = GetComponent<IActionContainer>();
        }

        private IEnumerator OnMouseEnter()
        {
            if (!CameraController.Instance.VisualEditing.Enabled)
                yield break;
            
            var amount = 0f;

            _renderer.material.SetFloat(ShouldHighlight, 1f);
            while (amount < 1f)
            {
                amount += 1.4f * Time.deltaTime;
                _renderer.material.SetFloat(HighlightAmount, Mathf.Min(amount, 1f));
                yield return null;
            }
            _renderer.material.SetFloat(HighlightAmount, 1f);
        }

        private void OnMouseExit()
        {
            _renderer.material.SetFloat(ShouldHighlight, 0f);
            _renderer.material.SetFloat(HighlightAmount, 0f);
        }

        private void OnMouseDown()
        {
            ProgrammableUIManager.Instance.OpenFor(_actionContainer);
        }
    }
}