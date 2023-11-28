using System;
using UnityEngine;

namespace Controller
{
    public class CameraController: MonoBehaviour
    {
        [SerializeField]
        private Transform player;

        private void Update()
        {
            // TODO: clamp camera position to level bounds
            transform.position = new Vector3(player.position.x, player.position.y, transform.position.z);
        }
    }
}