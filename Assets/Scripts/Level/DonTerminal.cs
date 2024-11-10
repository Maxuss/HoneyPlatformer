using System;
using System.Collections;
using System.Linq;
using Controller;
using DG.Tweening;
using Dialogue;
using DonDomain;
using Objects;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Level
{
    public class DonTerminal: MonoBehaviour, IInteractable, ISpawnPos
    {
        private SpriteRenderer _donRender;
        private static readonly int Distort = Shader.PropertyToID("_Distort");
        [SerializeField]
        private DialogueDefinition[] _dialogues;

        private void Start()
        {
            _donRender = transform.GetChild(0).GetComponent<SpriteRenderer>();
            _donRender.material.SetFloat(Distort, 0f);
        }

        private Tween _tween;
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player"))
                return;
            // TODO: toast popup
            if(_tween != null)
                _tween = DOTween.Sequence().Join(_tween).Join(DOTween.To(() => _donRender.material.GetFloat(Distort), val => _donRender.material.SetFloat(Distort, val),
                    1f, 0.34f)).OnComplete(() => _tween = null).Play();
            else
                _tween = DOTween.To(() => _donRender.material.GetFloat(Distort), val => _donRender.material.SetFloat(Distort, val),
                    1f, 0.34f).OnComplete(() => _tween = null).Play();
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag("Player"))
                return;
            // TODO: toast slideout
            if(_tween != null)
                _tween = DOTween.Sequence().Join(_tween).Join(DOTween.To(() => _donRender.material.GetFloat(Distort), val => _donRender.material.SetFloat(Distort, val),
                    0f, 0.34f)).OnComplete(() => _tween = null).Play();
            else
                _tween = DOTween.To(() => _donRender.material.GetFloat(Distort), val => _donRender.material.SetFloat(Distort, val),
                    0f, 0.34f).OnComplete(() => _tween = null).Play();
        }

        public void OnInteract()
        {
            StartCoroutine(Act());
        }

        private IEnumerator Act()
        {
            var lastScene = SceneManager.GetActiveScene().buildIndex;
            yield return DialogueManager.Instance.StartDialogue(_dialogues[Random.Range(0, _dialogues.Length)], true);
            // TODO: transition
            MusicManager.Instance.GetComponent<AudioSource>().DOFade(0f, 1f).Play();
            yield return PlayerController.Instance.FadeIn();
            EverythingPrefab.Instance.gameObject.SetActive(false);
            Destroy(EverythingPrefab.Instance.gameObject);
            SceneManager.LoadSceneAsync("donshop", LoadSceneMode.Single)!.completed += _ =>
            {
                SceneManager.GetSceneByName("donshop").GetRootGameObjects().First(it => it.TryGetComponent<DonShopManager>(out var _))
                    .GetComponent<DonShopManager>().lastScene = lastScene;
            };
        }

        public Transform SpawnPosition => transform;
    }
}