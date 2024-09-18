using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Controller
{
    public class ShakeController: MonoBehaviour
    {
        public static ShakeController Instance { get; private set; }
        
        private void Awake()
        {
            Instance = this;
        }
        
        [SerializeField] private float cameraShakeDecreaseFactor = 2.5f;
        [SerializeField] private float cameraShakeAmount = 0.1f;
        // coroutine
        public IEnumerator ShakeCamera(float duration)
        {
            var originalPos = transform.localPosition;
            while(duration > 0)
            {
                transform.localPosition = originalPos + Random.insideUnitSphere * cameraShakeAmount;
                duration -= Time.deltaTime * cameraShakeDecreaseFactor;
                yield return null;
            }
            transform.localPosition = originalPos;
        }
    }
}