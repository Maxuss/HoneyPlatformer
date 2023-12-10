using System;
using Controller;
using Cutscenes;
using Eflatun.SceneReference;
using Level;
using Program;
using Program.Channel;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using Utils;

namespace Objects
{
    [RequireComponent(typeof(Animator))]
    public class ExitDoor: MonoBehaviour, IActionContainer, IChannelReceiver
    {
        [SerializeField]
        private int nextLevel;
        [SerializeField]
        private AudioClip doorOpen;
        [SerializeField]
        private AudioClip doorClose;
        
        private Animator _anim;
        private DoorAction _action;
        private bool _state;

        private void Start()
        {
            _anim = GetComponent<Animator>();
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player"))
                return;
            
            // First object is always the grid
            var grid = SceneManager.GetSceneByBuildIndex(nextLevel).GetRootGameObjects()[0];
            var tilemap = grid.transform.GetChild(0).GetComponent<Tilemap>();

            var playerPos = PlayerController.Instance.transform.position;
            // TODO: weird behaviour on final builds??
            Debug.Log($"TILEMAP {tilemap} GRID: {grid}");
            Debug.Log($"LEVEL MANAGER {LevelManager.Instance}");
            LevelManager.Instance.SwitchLevel(tilemap.GetComponentInChildren<Tilemap>());

            StartCoroutine(this.CallbackCoroutine(
                PlayerController.Instance.AutonomousMove(playerPos + new Vector3(tilemap.cellSize.x * 3f, 0f)),
                () =>
                {
                    var scene = SceneManager.GetSceneByBuildIndex(nextLevel);
                    // Second object is always the entrance door
                    var rootObjects = scene.GetRootGameObjects();
                    var door = rootObjects[1];
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
                    var scene = SceneManager.GetSceneByBuildIndex(nextLevel);
                    var rootObjects = scene.GetRootGameObjects();
                    if (rootObjects.Length > 2 &&
                        rootObjects[2].TryGetComponent<ILevelEntranceCutscene>(out var cutscene))
                    {
                        ((MonoBehaviour) cutscene).StartCoroutine(Util.Delay(() => cutscene.StartCutscene(), .5f));
                    }
                })
            );
        }

        public void Unlock()
        {
            // door most likely already open
            if (SceneManager.sceneCount > 1)
                return;
            GetComponent<BoxCollider2D>().isTrigger = true;
            _anim.Play("UnlockDoor");
            SfxManager.Instance.Play(doorOpen, .5f);
            SceneManager.LoadSceneAsync(nextLevel, LoadSceneMode.Additive);
        }

        public void Lock()
        {
            // Scene already most likely loaded
            if (SceneManager.sceneCount <= 1)
                return;
            GetComponent<BoxCollider2D>().isTrigger = false;
            _anim.Play("LockDoor");
            SfxManager.Instance.Play(doorClose, .5f);
            SceneManager.UnloadSceneAsync(nextLevel);
        }

        public string Name => "Дверь выхода";
        public string Description => "Эту дверь вам необходимо открыть чтобы пройти в следующую комнату";

        public ActionInfo[] SupportedActions => new[]
        {
            new ActionInfo
            {
                ActionName = "Открыть",
                ActionDescription = "При получении сигнала 1 открывает, при получении 0 - закрывает.",
            },
            new ActionInfo
            {
                ActionName = "Закрыть",
                ActionDescription = "При получении сигнала 1 закрывает, при получении 0 - открывает."
            },
            new ActionInfo
            {
                ActionName = "Сменить состояние",
                ActionDescription =
                    "При получении сигнала 1 меняет состояние двери (закрытое->открытое и наоборот). При получении 0 ничего не делает."
            },
        };

        public ProgrammableType Type { get; } = ProgrammableType.Executor;
        public ActionData SelectedAction { get; set; }
        public void Begin(ActionData action)
        {
            _action = (DoorAction) Enum.ToObject(typeof(DoorAction), action.ActionIndex);
            RecalculateState();
        }

        private void RecalculateState()
        {
            switch (_action)
            {
                case DoorAction.OpenDoor when _state:
                    Unlock();
                    break;
                case DoorAction.CloseDoor when _state:
                    Lock();
                    break;
                case DoorAction.CloseDoor when !_state:
                    Unlock();
                    break;
                case DoorAction.OpenDoor when !_state:
                    Lock();
                    break;
                case DoorAction.SwapState when _state:
                    // door unlocked probably
                    if(SceneManager.sceneCount > 1)
                        Lock();
                    else
                        Unlock();
                    break;
            }
        }

        public void ReceiveBool(Transform source, bool b)
        {
            _state = b;
            RecalculateState();
        }

        public void ReceiveFloat(Transform source, float v)
        {
            // we do nothing with float values here
        }
    }

    public enum DoorAction
    {
        OpenDoor,
        CloseDoor,
        SwapState
    }
}