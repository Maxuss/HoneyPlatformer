using System;
using System.Linq;
using I18N;
using Utils;
using Random = UnityEngine.Random;

namespace Nodes.Trivia
{
    public class BinaryTriviaQuestion: ITriviaQuestion
    {
        public PreparedTriviaQuestion Prepare()
        {
            var randomNumber = Random.Range(100, 600);
            var correctHex = Convert.ToString(randomNumber, 2);
            var incorrect1 = Convert.ToString(Random.Range(120, 500), 2);
            var incorrect2 = Convert.ToString(randomNumber + 3, 16);
            var incorrect3 = Convert.ToString(randomNumber - 24, 16);
            var answers = new[] { incorrect1, incorrect2, incorrect3, correctHex };
            var shuffled = answers.ToList().Shuffle();
            var ans = shuffled.IndexOf(correctHex);
            return new PreparedTriviaQuestion()
            {
                Question = LocalizationManager.Instance.Translated("ui.node.trivia.question.binary").Replace("%num%", randomNumber.ToString()),
                Answers = shuffled.ToArray(),
                CorrectAnswer = ans,
                Reward = 124f * (randomNumber / 250f),
                TimeGiven = 30 * (randomNumber / 192f)
            };
        }

    }
}