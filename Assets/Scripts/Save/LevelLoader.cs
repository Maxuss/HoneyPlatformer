using System;
using System.Linq;
using Level;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

namespace Save
{
    public class LevelLoader: MonoBehaviour
    {
        [SerializeField]
        private GameObject everythingPrefab;

        public static LevelLoader Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        public void LoadLevel(int levelIdx)
        {
            var task = SceneManager.LoadSceneAsync(levelIdx, LoadSceneMode.Single);
            task.completed += op =>
            {
                var scene = SceneManager.GetActiveScene();
                var rootObjects = scene.GetRootGameObjects();
                var spawnPosObj = rootObjects.First(obj => obj.CompareTag("SpawnPos") || obj.CompareTag("EntranceDoor"))
                    .GetComponent<ISpawnPos>();
                var spawnPos = spawnPosObj.SpawnPosition;
                if (spawnPosObj is EntranceDoor door)
                {
                    door.GetComponent<Animator>().Play("EntranceDoor");
                    door.GetComponent<BoxCollider2D>().enabled = true;
                }
                
                var obj = Instantiate(everythingPrefab);
                var player=  obj.transform.Find("Player");
                player.position = spawnPos.position;
                
                var grid = rootObjects.First(oobj => oobj.CompareTag("Grid"));
            
                var tilemap = grid.transform.GetChild(0).GetComponent<Tilemap>();

                LevelManager.Instance.SwitchLevel(tilemap.GetComponentInChildren<Tilemap>());
            };
        }
    }
}