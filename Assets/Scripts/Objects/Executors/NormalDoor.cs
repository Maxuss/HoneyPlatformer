using System;
using System.Linq;
using Controller;
using Cutscenes;
using Level;
using Program;
using Program.Channel;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using Utils;

namespace Objects.Executors
{
    [RequireComponent(typeof(Animator))]
    public class NormalDoor: MonoBehaviour, IActionContainer, IChannelReceiver
    {
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

        public void Unlock()
        {
            _state = true;
            GetComponent<BoxCollider2D>().enabled = false;
            _anim.Play("UnlockDoor");
            SfxManager.Instance.Play(doorOpen, .5f);
        }

        public void Lock()
        {
            _state = false;
            GetComponent<BoxCollider2D>().enabled = true;
            _anim.Play("LockDoor");
            SfxManager.Instance.Play(doorClose, .5f);
        }

        public string Name => "Дверь";
        public string Description => "Эта дверь может стать вам препятствием";

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

        public ProgrammableType Type => ProgrammableType.Executor;
        [field: SerializeField]
        public ActionData SelectedAction { get; set; }
        public void Begin(ActionData action)
        {
            _action = (DoorAction) Enum.ToObject(typeof(DoorAction), action.ActionIndex);
            RecalculateState();
        }

        private void RecalculateState()
        {
            Debug.Log($"RECALCULATING {_action} {_state}");
            switch (_action)
            {
                case DoorAction.OpenDoor when _state:
                    if(gameObject != null)
                        Unlock();
                    break;
                case DoorAction.CloseDoor when _state:
                    if(gameObject != null)
                        Lock();
                    break;
                case DoorAction.CloseDoor when !_state:
                    if(gameObject != null)
                        Unlock();
                    break;
                case DoorAction.OpenDoor when !_state:
                    if(gameObject != null)
                        Lock();
                    break;
                case DoorAction.SwapState when _state:
                    // door unlocked probably
                    if(!_state)
                        Lock();
                    else
                        Unlock();
                    break;
            }
        }


        public void ReceiveBool(Transform source, bool b)
        {
            if (this == null || gameObject == null)
                return;
            if (_state == b)
                return;
            _state = b;
            RecalculateState();
        }

        public void ReceiveFloat(Transform source, float v)
        {
            // we do nothing with float values here
        }
    }
}