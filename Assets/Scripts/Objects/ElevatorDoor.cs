using System;
using UnityEngine;

namespace Objects
{
    public class ElevatorDoor: MonoBehaviour
    {
        private Animator _animator;

        private void Start()
        {
            _animator = GetComponent<Animator>();
        }

        public void Close()
        {
            _animator.Play("Close");
        }

        public void Open()
        {
            _animator.Play("Open");
        }
    }
}