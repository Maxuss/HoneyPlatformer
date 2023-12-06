using System;
using System.Collections;
using System.Collections.Generic;
using Level;
using Program.Channel;
using UnityEngine;

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

                yield return new WaitForSeconds(2.4f);
            }
        }

        public IEnumerator DestroyObject()
        {
            // TODO: disintegration effect
            SfxManager.Instance.Play(objectDestroySound);
            DestroyImmediate(_spawnedObject);
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
            SfxManager.Instance.Play(objectCreateSound);
            foreach(var particle in particles)
                particle.Play();
            
            yield return new WaitForSeconds(.8f);
            
            foreach(var particle in particles)
                particle.Stop();

            _spawnedObject = Instantiate(objectPrefab, spawnPosition.position, Quaternion.identity);
        }
    }
}