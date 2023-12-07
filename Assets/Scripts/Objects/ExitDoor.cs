using Controller;
using Eflatun.SceneReference;
using Level;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using Utils;

namespace Objects
{
    [RequireComponent(typeof(Animator))]
    public class ExitDoor: MonoBehaviour
    {
        [SerializeField]
        private SceneReference nextLevel;
        [SerializeField]
        private AudioClip doorOpen;
        [SerializeField]
        private AudioClip doorClose;
        
        private Animator _anim;


        private void Start()
        {
            _anim = GetComponent<Animator>();
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player"))
                return;
            
            // First object is always the grid
            var grid = nextLevel.LoadedScene.GetRootGameObjects()[0];
            var tilemap = grid.GetComponentInChildren<Tilemap>();

            var playerPos = PlayerController.Instance.transform.position;
            
            LevelManager.Instance.SwitchLevel(tilemap.GetComponentInChildren<Tilemap>());

            StartCoroutine(this.CallbackCoroutine(
                PlayerController.Instance.AutonomousMove(playerPos + new Vector3(tilemap.cellSize.x * 3f, 0f)),
                () =>
                {
                    var scene = nextLevel.LoadedScene;
                    // Second object is always the entrance door
                    var door = scene.GetRootGameObjects()[1];
                    door.GetComponent<Animator>().Play("LockDoor");
                    door.GetComponent<BoxCollider2D>().enabled = true;
                    SfxManager.Instance.Play(doorClose, .5f);
                }
            ));
            StartCoroutine(this.CallbackCoroutine(
                CameraController.Instance.TransitionToPoint(playerPos + new Vector3(tilemap.cellSize.x * 11f, 0f)),
                () =>
                {
                    _anim.StopPlayback();
                    var unloaded = SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(0));
                    unloaded.completed += _ =>
                    {
                        LaserManager.Instance.Reload();
                    };
                })
            );
        }

        public void Unlock()
        {
            GetComponent<BoxCollider2D>().isTrigger = true;
            _anim.Play("UnlockDoor");
            SfxManager.Instance.Play(doorOpen, .5f);
            SceneManager.LoadSceneAsync(nextLevel.BuildIndex, LoadSceneMode.Additive);
        }
    }
}