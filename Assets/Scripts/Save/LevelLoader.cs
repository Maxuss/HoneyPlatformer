using System;
using System.Collections;
using System.Linq;
using Controller;
using Cutscenes;
using Igroprom;
using Level;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using Utils;

namespace Save
{
    public class LevelLoader: MonoBehaviour
    {
        [SerializeField]
        private GameObject everythingPrefab;

        public int LevelIndex { get; private set; }

        public static LevelLoader Instance { get; private set; }

        public enum LevelLocation
        {
            LivingQuarters,
            MainArea,
            OldArea,
            OldestArea,
            CaptainArea,
            TopArea,
            HiveArea
        }

        public static LevelLocation[] LEVEL_LOCS =
        {
            LevelLocation.LivingQuarters,
            LevelLocation.LivingQuarters,
            LevelLocation.LivingQuarters,
            LevelLocation.LivingQuarters,
            LevelLocation.LivingQuarters,
            LevelLocation.LivingQuarters,
            LevelLocation.LivingQuarters,
            LevelLocation.MainArea,
            LevelLocation.MainArea,
            LevelLocation.MainArea,
            LevelLocation.MainArea,
            LevelLocation.OldArea,
            LevelLocation.OldArea,
            LevelLocation.OldArea,
            LevelLocation.OldArea,
            LevelLocation.MainArea,
            LevelLocation.MainArea,
            LevelLocation.MainArea,
            LevelLocation.MainArea,
            LevelLocation.MainArea,
            LevelLocation.MainArea,
            LevelLocation.MainArea,
            LevelLocation.MainArea,
            LevelLocation.MainArea,
            LevelLocation.MainArea,
            LevelLocation.MainArea,
            LevelLocation.MainArea,
            LevelLocation.OldestArea,
            LevelLocation.OldestArea,
            LevelLocation.OldestArea,
            LevelLocation.OldestArea,
            LevelLocation.OldestArea,
            // TODO: finish
        };

        private void Awake()
        {
            Instance = this;
        }

        public void LoadLevel(int levelIdx, bool searchPedestal = false)
        {
            var task = SceneManager.LoadSceneAsync(levelIdx, LoadSceneMode.Single);
            task.completed += op =>
            {
                var scene = SceneManager.GetActiveScene();
                var rootObjects = scene.GetRootGameObjects();
                var watcher = rootObjects.FirstOrDefault(it => it.TryGetComponent<WatcherDoor>(out var cmp));
                if (watcher != null)
                    watcher.GetComponent<WatcherDoor>().beginTime = Time.realtimeSinceStartup;
                
                var spawnPosObj = rootObjects.First(obj => obj.CompareTag("SpawnPos") || obj.CompareTag("EntranceDoor"))
                    .GetComponent<ISpawnPos>();
                var spawnPos = searchPedestal ? rootObjects.First(obj => obj.TryGetComponent<DonTerminal>(out _)).GetComponent<ISpawnPos>().SpawnPosition : spawnPosObj.SpawnPosition;
                if (spawnPosObj is EntranceDoor door)
                {
                    door.GetComponent<Animator>().Play("EntranceDoor");
                    door.GetComponent<BoxCollider2D>().enabled = true;
                }
                
                var obj = Instantiate(everythingPrefab);
                var player=  obj.transform.Find("Player");
                player.position = spawnPos.position;
                var controller = player.GetComponent<PlayerController>();
                controller.BlackOut();
                controller.StartCoroutine(controller.FadeOut());
                var grid = rootObjects.First(oobj => oobj.CompareTag("Grid"));
            
                var tilemap = grid.transform.GetChild(0).GetComponent<Tilemap>();

                LevelManager.Instance.SwitchLevel(tilemap.GetComponentInChildren<Tilemap>());
                
                var cutscene = rootObjects.FirstOrDefault(oobj => oobj.CompareTag("Cutscene"));
                if (cutscene != null)
                {
                    cutscene.GetComponent<MonoBehaviour>().StartCoroutine(Util.Delay(() => cutscene.GetComponent<ILevelEntranceCutscene>().StartCutscene(), .5f));
                }
                MusicManager.Instance.NextAmbientTrack();
                
                SaveManager.CurrentState.LevelIndex = scene.buildIndex;
                SaveManager.CurrentState.LevelName = scene.name;
                SaveManager.SaveGame(true);
            };
        }

        public IEnumerator TransitionLevel(int levelIdx)
        {
            yield return PlayerController.Instance.FadeIn();
            var task = SceneManager.LoadSceneAsync(levelIdx, LoadSceneMode.Single);
            LevelIndex = levelIdx;
            task.completed += op =>
            {
                PlayerController.Instance.BlackOut();
                var scene = SceneManager.GetActiveScene();
                var rootObjects = scene.GetRootGameObjects();
                var watcher = rootObjects.FirstOrDefault(it => it.TryGetComponent<WatcherDoor>(out var cmp));
                if (watcher != null)
                    watcher.GetComponent<WatcherDoor>().beginTime = Time.realtimeSinceStartup;
                var spawnPosObj = rootObjects.First(obj => obj.CompareTag("SpawnPos") || obj.CompareTag("EntranceDoor"))
                    .GetComponent<ISpawnPos>();
                var spawnPos = spawnPosObj.SpawnPosition;
                if (spawnPosObj is EntranceDoor door)
                {
                    door.GetComponent<Animator>().Play("EntranceDoor");
                    door.GetComponent<BoxCollider2D>().enabled = true;
                }

                var player = PlayerController.Instance.transform;
                player.position = spawnPos.position;
                LevelManager.Instance.StartCoroutine(PlayerController.Instance.FadeOut());
                var grid = rootObjects.First(oobj => oobj.CompareTag("Grid"));
            
                var tilemap = grid.transform.GetChild(0).GetComponent<Tilemap>();

                LevelManager.Instance.SwitchLevel(tilemap.GetComponentInChildren<Tilemap>());
                
                var cutscene = rootObjects.FirstOrDefault(oobj => oobj.CompareTag("Cutscene"));
                if (cutscene != null)
                {
                    CameraController.Instance.ExitProgramMode();
                    cutscene.GetComponent<ILevelEntranceCutscene>().StartCutscene();
                }
                
                SaveManager.CurrentState.LevelIndex = scene.buildIndex;
                SaveManager.CurrentState.LevelName = scene.name;
                SaveManager.SaveGame(true);
            };
        }
    }
}