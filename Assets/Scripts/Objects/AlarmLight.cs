using System;
using UnityEngine;

namespace Objects
{
    public class AlarmLight: MonoBehaviour
    {
        [SerializeField]
        private Transform light1;
        [SerializeField]
        private Transform light2;
        [SerializeField]
        private float rotationSpeed;

        private float _rotationAngles;

        private void Update()
        {
            light1.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
            light2.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
        }
    }
}