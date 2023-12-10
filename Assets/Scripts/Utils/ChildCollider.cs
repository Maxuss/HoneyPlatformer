using System;
using UnityEngine;

namespace Utils
{
    public class ChildCollider: MonoBehaviour
    {
        private IParentCollisionHandler _parent;
        
        private void Start()
        {
            _parent = gameObject.GetComponentInParent<IParentCollisionHandler>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            _parent.OnChildTriggerEnter(other);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            _parent.OnChildTriggerExit(other);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            _parent.OnChildTriggerStay(other);
        }
    }

    public interface IParentCollisionHandler
    {
        public void OnChildTriggerEnter(Collider2D other)
        {
            
        }

        public void OnChildTriggerExit(Collider2D other)
        {
            
        }

        public void OnChildTriggerStay(Collider2D other)
        {
            
        }
    }
}