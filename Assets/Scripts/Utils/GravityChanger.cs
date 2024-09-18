using System;
using Controller;
using UnityEngine;

namespace Utils
{
    public class GravityChanger: MonoBehaviour
    {
        private Rigidbody2D _player;
        private void Start()
        {
            _player = PlayerController.Instance.GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            if (!Input.GetKeyDown(KeyCode.G))
                return;
            _player.gravityScale = -1;
        }
    }
}