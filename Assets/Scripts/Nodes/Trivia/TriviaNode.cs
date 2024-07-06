using System;
using System.Collections.Generic;
using System.Linq;
using Controller;
using Level;
using Save;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using Random = UnityEngine.Random;

namespace Nodes.Trivia
{
    public class TriviaNode: MonoBehaviour
    {
        [SerializeField]
        private AudioClip incorrectSfx;
        [SerializeField]
        private Button[] answers;
        [SerializeField]
        private TMP_Text question;

        [SerializeField]
        private Image bar;

        private List<ITriviaQuestion> _questions = new()
        {
            new PreparedTriviaQuestion()
            {
                TimeGiven = 3f,
                Question = "Где вы находитесь?",
                Answers = new[] {"Станция МКС", "Станция 'Академия'", "Станция BRZ", "Корабль Гигапасека"},
                CorrectAnswer = 2,
                Reward = 50f
            },
            new HexTriviaQuestion(),
            new BinaryTriviaQuestion(),
            new PreparedTriviaQuestion
            {
                TimeGiven = 3f,
                Question = "Как зовут единственного творца на станции?",
                Answers = new[] {"Дониил", "Марсель", "Мефодий", "Сапсан"},
                CorrectAnswer = 0,
                Reward = 66f
            },
            new PreparedTriviaQuestion
            {
                TimeGiven = 4.5f,
                Question = "Кого из экипажа избегает Мефодий?",
                Answers = new[] {"Дониила", "Сапсана", "Евдокима", "Олега"},
                CorrectAnswer = 0,
                Reward = 70f
            }
        };

        private float _timeLeft;
        private float _graceTime = 1f;

        private PreparedTriviaQuestion _chosenQuestion;

        private void Start()
        {
            _chosenQuestion = _questions[Random.Range(0, _questions.Count)].Prepare();
            var origCorrect = _chosenQuestion.Answers[_chosenQuestion.CorrectAnswer];
            var shuffledAnswers = _chosenQuestion.Answers.Shuffle();
            _chosenQuestion.CorrectAnswer = shuffledAnswers.IndexOf(origCorrect);
            _chosenQuestion.Answers = shuffledAnswers.ToArray();
            
            _timeLeft = _chosenQuestion.TimeGiven;
            question.text = _chosenQuestion.Question;
            for (var i = 0; i < 4; i++)
            {
                var ansBtn = answers[i];
                ansBtn.GetComponentInChildren<TMP_Text>().text = _chosenQuestion.Answers[i];
                ansBtn.onClick.RemoveAllListeners();
                if(_chosenQuestion.CorrectAnswer == i)
                    ansBtn.onClick.AddListener(() =>
                    {
                        SaveManager.CurrentState.Currency += Mathf.RoundToInt(_chosenQuestion.Reward *
                                                                              (1 + (_timeLeft /
                                                                                  _chosenQuestion.TimeGiven - 0.5f)));
                        NodeManager.Instance.SelectedNode.MarkCalibrated();
                    });
                else
                {
                    ansBtn.onClick.AddListener(() =>
                    {
                        SfxManager.Instance.Play(incorrectSfx);
                        NodeManager.Instance.Close();
                        ToastManager.Instance.ShowToast("Неверный ответ!");
                    });
                }
            }
        }

        private void Update()
        {
            if (_graceTime > 0f)
            {
                _graceTime -= Time.deltaTime;
                return;
            }
            _timeLeft -= Time.deltaTime;
            var barWidth = (_timeLeft / _chosenQuestion.TimeGiven);
            bar.fillAmount = barWidth;
            if (_timeLeft <= 0f)
            {
                SfxManager.Instance.Play(incorrectSfx, .4f);
                NodeManager.Instance.Close();
                ToastManager.Instance.ShowToast("Время истекло!");
            }
        }
    }
}