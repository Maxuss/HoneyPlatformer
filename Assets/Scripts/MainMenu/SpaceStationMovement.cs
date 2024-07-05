using System;
using UnityEngine;

namespace MainMenu
{
    public class SpaceStationMovement: MonoBehaviour
    {
        [SerializeField]
        private Transform spaceStation;
        [SerializeField]
        private float speed;

        private float _yaw = 0.0f;
        private float _pitch = 0.0f;        
        
        private void Update()
        {
            _yaw += speed * Input.GetAxis("Mouse X");
            _pitch -= speed * Input.GetAxis("Mouse Y");
            _yaw = Mathf.Clamp(_yaw, -1f, 10f);
            _pitch = Mathf.Clamp(_pitch, -1f, 10f);
            transform.eulerAngles = new Vector3(_pitch, _yaw, 0f);
        }
    }
}