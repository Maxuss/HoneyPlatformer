using System;
using UnityEngine;

namespace Utils
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class ParallaxBackground: MonoBehaviour
    {
        private float length, startPos;
        
        [SerializeField]
        private Transform cameraTransform;
        [SerializeField]
        private float transformModifier;

        public void Start()
        {
            startPos = transform.position.x;
            length = GetComponent<SpriteRenderer>().bounds.size.x;
        }

        private void FixedUpdate()
        {
            var tmp = cameraTransform.position.x * (1 - transformModifier);
            var dist = (cameraTransform.position.x * transformModifier);

            transform.position = new Vector3(startPos + dist, cameraTransform.position.y + 5f, transform.position.z);

            if (tmp > startPos + length) startPos += length;
            else if (tmp < startPos - length) startPos -= length;
        }
    }
}