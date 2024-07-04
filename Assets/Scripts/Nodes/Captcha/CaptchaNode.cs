using System;
using System.Collections;
using System.Linq;
using Level;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using Random = UnityEngine.Random;

namespace Nodes.Captcha
{
    public class CaptchaNode: MonoBehaviour
    {
        [SerializeField]
        private Gradient textGradient;
        [SerializeField]
        private Transform captchaContainer;
        [SerializeField]
        private TMP_InputField input;

        [SerializeField]
        private GameObject textTemplate;
        [SerializeField]
        private GameObject letterTemplate;
        [SerializeField]
        private AudioClip incorrectSfx;
        
        private Button _btn;

        private string _chosenCaptcha = "";

        private void Start()
        {
            _btn = GetComponent<Button>();
            _btn.onClick.AddListener(Check);
            StartCoroutine(GenerateCaptcha());
        }

        private IEnumerator GenerateCaptcha()
        {
            var tmp = Instantiate(textTemplate, captchaContainer);
            yield return new WaitForSeconds(1 + Random.Range(0.1f, 0.4f));
            Destroy(tmp);
            var letters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var chosenLetters = Enumerable.Range(0, 6)
                .Select(each => letters[Random.Range(0, letters.Length)]).ToArray();
            var idx = 0;
            foreach (var letter in chosenLetters)
            {
                var obj = Instantiate(letterTemplate, captchaContainer);
                var txt = obj.GetComponent<TMP_Text>();
                txt.text = letter.ToString();
                txt.color = textGradient.Evaluate(Random.value);
                txt.fontSize = Random.Range(70, 110);
                txt.transform.rotation = Quaternion.Euler(new Vector3(Random.Range(-15, 15), Random.Range(-15, 15),
                    Random.Range(-15, 15)));
                txt.transform.localPosition = new Vector3(-105 + idx * (Random.Range(0.95f, 1.1f) * 45), Random.Range(-30f, 30f), 0f);
                idx++;
            }

            _chosenCaptcha = new string(chosenLetters);
            
        }

        public void Check()
        {
            if (_chosenCaptcha == "")
                return;
            _btn.interactable = false;
            _btn.GetComponentInChildren<TMP_Text>().text = "Идет проверка...";
            StartCoroutine(CheckCoroutine());
        }

        private IEnumerator CheckCoroutine()
        {
            yield return new WaitForSeconds(1f);
            if (input.text.Trim() == _chosenCaptcha)
            {
                NodeManager.Instance.SelectedNode.MarkCalibrated();
                NodeManager.Instance.Close();
            }
            else
            {
                NodeManager.Instance.Close();
                SfxManager.Instance.Play(incorrectSfx, .4f);
                ToastManager.Instance.ShowToast("Неверная капча!");
            }
        }
    }
}