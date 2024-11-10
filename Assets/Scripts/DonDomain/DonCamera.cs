using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace DonDomain
{
    [RequireComponent(typeof(Camera))]
    public class DonCamera : MonoBehaviour {
        public Shader crtShader;
        [Range(1.0f, 10.0f)]
        public float curvature = 1.0f;

        [Range(1.0f, 100.0f)]
        public float vignetteWidth = 30.0f;

        private Material crtMat;
        private static readonly int Curvature = Shader.PropertyToID("_Curvature");
        private static readonly int VignetteWidth = Shader.PropertyToID("_VignetteWidth");

        void Start() {
            crtMat ??= new Material(crtShader);
            crtMat.hideFlags = HideFlags.HideAndDontSave;
        }

        void OnRenderImage(RenderTexture source, RenderTexture destination) {
            crtMat.SetFloat(Curvature, curvature);
            crtMat.SetFloat(VignetteWidth, vignetteWidth);
            Graphics.Blit(source, destination, crtMat);
        }
    }
}