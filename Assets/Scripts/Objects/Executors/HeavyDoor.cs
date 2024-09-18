using System;
using Controller;
using Level;
using Program;
using Program.Channel;
using UnityEngine;
using Utils;

namespace Objects.Executors
{
    [RequireComponent(typeof(Animator))]
    public class HeavyDoor: MonoBehaviour, IActionContainer, IChannelReceiver
    {
        [SerializeField]
        private AudioClip doorOpen;
        [SerializeField]
        private AudioClip doorClose;
        private Animator _anim;
        private float _remainingTime;
        [SerializeField]
        private float totalTime = 5f;

        private SpriteRenderer _sr;

        private bool _locked;
        private static readonly int Remaining = Shader.PropertyToID("_Remaining");

        private void Start()
        {
            _sr = GetComponent<SpriteRenderer>();
            _anim = GetComponent<Animator>();
            _locked = true;
            _remainingTime = 0;
            GetComponent<BoxCollider2D>().enabled = true;
            _anim.Play("HeavyDoorLocked");
        }

        public void Unlock()
        {
            GetComponent<BoxCollider2D>().enabled = false;
            _anim.Play("UnlockHeavyDoor");
            SfxManager.Instance.Play(doorOpen, .5f);
            _locked = false;
        }

        public void Lock()
        {
            GetComponent<BoxCollider2D>().enabled = true;
            _anim.Play("LockHeavyDoor");
            SfxManager.Instance.Play(doorClose, .5f);
            StartCoroutine(Util.Delay(() => StartCoroutine(ShakeController.Instance.ShakeCamera(0.3f)), 0.2f));
            _locked = true;
        }

        public string Name => "obj.heavy_door.name";
        public string Description => "obj.heavy_door.desc";

        public ActionInfo[] SupportedActions => new[]
        {
            new ActionInfo
            {
                ActionName = "Открыть и удерживать",
                ActionDescription = "При получении сигнала 1 открывает и удерживает открытой",
            },
        };

        public ProgrammableType Type => ProgrammableType.Executor;
        [field: SerializeField]
        public ActionData SelectedAction { get; set; }
        public void Begin(ActionData action)
        {
        }

        public void ReceiveBool(Transform source, bool b)
        {
            if (this == null || gameObject == null)
                return;
            if (!b)
                return;
            _remainingTime = totalTime;
            if(_locked)
                Unlock();
            
        }

        public void ReceiveFloat(Transform source, float v)
        {
            // we do nothing with float values here
        }

        private void Update()
        {
            if (_remainingTime <= 0 && !_locked)
            {
                Lock();
            }
            else
            {
                _remainingTime -= Time.deltaTime;
                _sr.material.SetFloat(Remaining, _remainingTime / totalTime);
            }
        }
    }
}