using System;
using System.Collections;
using System.Collections.Generic;
using Level;
using Program.Channel;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

namespace Objects
{
    public class ObjectSpawner : MonoBehaviour, IChannelReceiver
    {
        // TODO: different objects to program!!
        [SerializeField]
        private GameObject objectPrefab;
        
        [SerializeField]
        private AudioClip objectCreateSound;
        [SerializeField]
        private AudioClip objectDestroySound;
        [SerializeField]
        private Transform spawnPosition;
        [SerializeField]
        private ParticleSystem[] particles;
        
        private GameObject _spawnedObject;
        private static readonly int Amount = Shader.PropertyToID("_Amount");
        private static readonly int StartPosition = Shader.PropertyToID("_StartPosition");

        private void Start()
        {
            foreach(var particle in particles)
                particle.Stop();
        }

        public void ReceiveBool(bool b)
        {
            if(b)
                SpawnObject();
            else
                StartCoroutine(DestroyObject());
        }

        public void ReceiveFloat(float v)
        {
            if (v > 1f)
            {
                StartCoroutine(SpawnNumObjects(Mathf.FloorToInt(v)));
            }
            else if(v > 0f)
            {
                SpawnObject();
            }
        }

        private IEnumerator SpawnNumObjects(int count)
        {
            while (count > 0)
            {
                count--;
                
                SpawnObject();

                yield return new WaitForSeconds(3f);
            }
        }

        public IEnumerator DestroyObject()
        {
            var objRenderer = _spawnedObject.GetComponent<Renderer>();

            var destructionAmount = 0f;
            objRenderer.material.SetVector(StartPosition, Vector2.zero);

            var objCollider = _spawnedObject.GetComponent<Collider2D>();
            objCollider.enabled = false;
            var rb = _spawnedObject.GetComponent<Rigidbody2D>();
            rb.gravityScale = -1f;

            SfxManager.Instance.Play(objectDestroySound, .5f);
            while (destructionAmount < 0.9f)
            {
                destructionAmount += 0.9f * Time.fixedDeltaTime;
                objRenderer.material.SetFloat(Amount, destructionAmount);

                yield return null;
            }
            
            _spawnedObject.SetActive(false);
            var obj = _spawnedObject;
            // the object blinks for a frame if destroyed immediately, so delay the destruction
            StartCoroutine(Util.DelayFrames(() => DestroyImmediate(obj), 1));
            yield return null;
        }
        
        [ContextMenu("Spawn Object")]
        public void SpawnObject()
        {
            StartCoroutine(DestroyObject());
            StartCoroutine(DelayedSpawnObject());
        }

        private IEnumerator DelayedSpawnObject()
        {
            SfxManager.Instance.Play(objectCreateSound, .5f);
            
            _spawnedObject = Instantiate(objectPrefab, spawnPosition.position, Quaternion.identity);
            var objRenderer = _spawnedObject.GetComponent<Renderer>();
            var rb = _spawnedObject.GetComponent<Rigidbody2D>();
            rb.isKinematic = true;
            objRenderer.material.SetVector(StartPosition, Vector2.zero);
            objRenderer.material.SetFloat(Amount, 1f);

            var amount = 1f;
            
            foreach(var particle in particles)
                particle.Play();

            while (amount >= 0f)
            {
                amount -= .8f * Time.fixedDeltaTime;
                objRenderer.material.SetFloat(Amount, amount);
                yield return null;
            }

            rb.isKinematic = false;
            rb.velocity += (Vector2) (-transform.up * 6f);
            objRenderer.material.SetFloat(Amount, 0f);

            foreach (var particle in particles)
                particle.Stop();
        }
    }
}