using System;
using Controller;
using Eflatun.SceneReference;
using Level;
using Program;
using Program.Action;
using Program.Trigger;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using Utils;

namespace Objects
{
    [RequireComponent(typeof(Animator))]
    public class ExitDoor: Programmable
    {
        [SerializeField]
        private SceneReference nextLevel;
        
        private Animator _anim;

        public override ITrigger[] ApplicableTriggers => new ITrigger[] { new TimeoutTrigger(0.5f) };
        public override IAction[] ApplicableActions => new[] { UnlockAction };

        protected override void Init()
        {
            _anim = GetComponent<Animator>();
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player"))
                return;
            // TODO: camera movement to the next level
            Debug.Log("TRIGGER PASSED");
            var obj = nextLevel.LoadedScene.GetRootGameObjects()[0];
            var tilemap = obj.GetComponentInChildren<Tilemap>();

            var playerPos = PlayerController.Instance.transform.position;
            PlayerController.Instance.transform.position += new Vector3(tilemap.cellSize.x * 2, 0f);
            LevelManager.Instance.SwitchLevel(tilemap.GetComponentInChildren<Tilemap>());
            
            StartCoroutine(this.CallbackCoroutine(
                CameraController.Instance.TransitionToPoint(playerPos + new Vector3(tilemap.cellSize.x * 11f, 0f)),
                () =>
                {
                    _anim.StopPlayback();
                    SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(0));
                })
            );
        }

        public static IAction UnlockAction => new DelegatedAction("Unlock door", obj =>
        {
            var door = obj as ExitDoor;
            door!.GetComponent<BoxCollider2D>().isTrigger = true;
            door!._anim.Play("UnlockDoor"); 
            SceneManager.LoadSceneAsync(door.nextLevel.BuildIndex, LoadSceneMode.Additive);
        });
    }
}