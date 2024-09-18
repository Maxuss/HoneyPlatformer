using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Controller;
using Objects.Emitters;
using Program;
using Program.Channel;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

namespace Objects.Executors
{
    public class SpeakerBox: MonoBehaviour, IActionContainer, IChannelReceiver
    {
        public string Name => "obj.speaker.name";
        public string Description => "obj.speaker.desc";
        public ProgrammableType Type => ProgrammableType.Executor;
        public ActionData SelectedAction { get; set; }

        [SerializeField]
        private float radius = 10;

        [SerializeField]
        private LayerMask mask;
        private Vector2 _oldPos;
        private Vector2 _movedAmount;
        private IChannelSender _parent;
        private SpriteRenderer _soundWave;
        private Transform _soundWaveTf;
        private AudioSource _as;
        [SerializeField]
        private AudioClip soundClip;

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(transform.position, radius);
        }

        private void Start()
        {
            _as = GetComponent<AudioSource>();
            _soundWaveTf = transform.GetChild(0);
            _soundWave = _soundWaveTf.GetComponent<SpriteRenderer>();
            _soundWaveTf.gameObject.SetActive(false);
            _oldPos = transform.position;
            _parent = SceneManager.GetActiveScene().GetRootGameObjects().First(it => it.TryGetComponent<IChannelSender>(out var sender) && sender.ConnectedRx.Contains(this)).GetComponent<IChannelSender>();
        }

        private void FixedUpdate()
        {
            if (!transform.hasChanged)
                return;

            _movedAmount += (transform.position.XY() - _oldPos).Abs();
            _oldPos = transform.position;
            if (_movedAmount.sqrMagnitude >= 2)
            {
                CameraController.Instance.VisualEditing.PollLineRender(_parent);
                _movedAmount = new Vector2();
            }
        }

        public void Begin(ActionData action)
        {
            
        }

        private bool _occupied = false;

        public void ReceiveBool(Transform source, bool b)
        {
            if (!b || _occupied)
                return;

            _occupied = true;
            // TODO: sound !

            StartCoroutine(ScaleUp());
        }

        private IEnumerator ScaleUp()
        {
            _as.PlayOneShot(soundClip, 0.4f);
            _soundWaveTf.gameObject.SetActive(true);
            _soundWave.color = new Color(1f, 1f, 1f, 1f);
            var size = 0f;
            var begin = Time.time;
            while (size < radius * .25f)
            {
                size = Mathf.SmoothStep(0f, radius * .25f, (Time.time - begin) / 0.7f);
                _soundWaveTf.localScale = new Vector3(size, size, 1);
                yield return null;
            }

            _soundWaveTf.localScale = new Vector3(radius * .25f, radius * .25f, 1f);
            
            var colliders = Physics2D.OverlapCircleAll(transform.position, radius).ToList();
            var fittingColliders = colliders.Where(it =>
                it.gameObject.TryGetComponent<SoundReceiver>(out var cmp) &&
                !(cmp.Rx is SpeakerBox box && box == this));
            foreach (var cl in fittingColliders)
            {
                var dir = transform.position - cl.transform.position;
                dir.z = 0;
                dir.Normalize();
                var hit = Physics2D.Raycast(cl.transform.position, dir.normalized, radius, mask);
                Debug.Log($"HIT: {hit.transform.gameObject}");
                if (hit.transform.gameObject != gameObject)
                    continue;
                cl.gameObject.GetComponent<SoundReceiver>().Impulse();
            }

            yield return new WaitForSeconds(0.2f);

            var opacity = 1f;
            while (opacity > 0f)
            {
                opacity -= Time.deltaTime;
                _soundWave.color = new Color(1f, 1f, 1f, opacity);
                yield return null;
            }

            _occupied = false;
            _soundWaveTf.gameObject.SetActive(false);
        }

        public void ReceiveFloat(Transform source, float v)
        {
        }
        
        public ActionInfo[] SupportedActions { get; } = {
            new()
            {
                ActionName = "Издать звуковой сигнал",
                ActionDescription = "Издает звуковой импульс, который распространится на радиус 10 метров",
            },
        };
    }
}