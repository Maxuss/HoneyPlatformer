using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;

namespace Cutscenes
{
    public class BlinkingLight: MonoBehaviour
    {
        private Light2D _light;

        [SerializeField]
        private float maxInterval = 1f;

        private float _targetIntensity;
        private float _lastIntensity;
        private float _interval;
        private float _timer;

        
        private void Start()
        {
            _light = GetComponentInChildren<Light2D>();
        }

        void Update()
        {
            _timer += Time.deltaTime;

            if (_timer > _interval)
            {
                _lastIntensity = _light.intensity;
                _targetIntensity = Random.Range(0.5f, 1f);
                _timer = 0;
                _interval = Random.Range(0, maxInterval);
            }

            _light.intensity = Mathf.Lerp(_lastIntensity, _targetIntensity, _timer / _interval);
        }
    }
}