using System;
using System.Collections;
using System.Linq;
using Controller;
using Cutscenes;
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

        public static LevelLoader Instance { get; private set; }

        public static string[] LEVEL_NAMES = new []
        {
            "Кабина Сапсана",
            "Тусклый коридор",
            "Заевшая дверь",
            "Встреча с Мефодием",
            "Кибер-осы!",
            "Двойная проблема",
            "Брошеная лаборатория",
            "Торговец",
            "Лазерная тюрьма",
            "Спектр",
            "Цех производства ос",
            "Сверхзвуковой лифт",
            "Цех конструкторов",
            "Непредвиденные последствия",
            "Яркий коридор",
            "Лазерный мост",
            "В столовой",
            "Капитанский мостик",
            "ГАЛИЛЕО x 3",
            "Капитанский кошмар",
            "Капитанское принятие",
            "Цель близка",
            "Блистающий коридор",
            "Хаотичный спектр",
            "Логическая энигма",
            "Последний рывок",
            "In Utero"
        };

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
                
                var cutscene = rootObjects.FirstOrDefault(oobj => oobj.CompareTag("Cutscene"));
                if (cutscene != null)
                {
                    cutscene.GetComponent<MonoBehaviour>().StartCoroutine(Util.Delay(() => cutscene.GetComponent<ILevelEntranceCutscene>().StartCutscene(), .5f));
                }
            };
        }

        public IEnumerator TransitionLevel(int levelIdx)
        {
            yield return PlayerController.Instance.FadeIn();
            var task = SceneManager.LoadSceneAsync(levelIdx, LoadSceneMode.Single);
            task.completed += op =>
            {
                PlayerController.Instance.BlackOut();
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

                var player = PlayerController.Instance.transform;
                player.position = spawnPos.position;
                PlayerController.Instance.StartCoroutine(PlayerController.Instance.FadeOut());
                var grid = rootObjects.First(oobj => oobj.CompareTag("Grid"));
            
                var tilemap = grid.transform.GetChild(0).GetComponent<Tilemap>();

                LevelManager.Instance.SwitchLevel(tilemap.GetComponentInChildren<Tilemap>());
                
                var cutscene = rootObjects.FirstOrDefault(oobj => oobj.CompareTag("Cutscene"));
                if (cutscene != null)
                {
                    cutscene.GetComponent<ILevelEntranceCutscene>().StartCutscene();
                }
            };
        }
    }
}