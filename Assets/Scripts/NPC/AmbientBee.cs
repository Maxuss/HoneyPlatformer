using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace NPC
{
    public class AmbientBee: MonoBehaviour
    {
        public void Start()
        {
            var animator = GetComponent<Animator>();
            animator.StopPlayback();
            animator.Play("Bee", 0, Random.value);
        }
    }
}