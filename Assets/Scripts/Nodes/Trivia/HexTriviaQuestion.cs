using System;
using System.Linq;
using Utils;
using Random = UnityEngine.Random;

namespace Nodes.Trivia
{
    public class HexTriviaQuestion: ITriviaQuestion
    {
        public PreparedTriviaQuestion Prepare()
        {
            var randomNumber = Random.Range(100, 600);
            var correctHex = randomNumber.ToString("X");
            var incorrect1 = Convert.ToString(Random.Range(120, 500), 7);
            var incorrect2 = correctHex.ToCharArray();
            incorrect2[Random.Range(0, 3)] = new[] { 'G', '0', 'E', '9', 'I' }[Random.Range(0, 5)];
            var incorrect3 = (randomNumber + 24).ToString("X").Replace('7', 'F');
            var answers = new[] { incorrect1, new(incorrect2), incorrect3, correctHex };
            var shuffled = answers.ToList().Shuffle();
            var ans = shuffled.IndexOf(correctHex);
            return new PreparedTriviaQuestion()
            {
                Question = $"Переведите <color=yellow>{randomNumber}</color> в шестнадцатеричную систему счисления",
                Answers = shuffled.ToArray(),
                CorrectAnswer = ans,
                Reward = 144f * (randomNumber / 250f),
                TimeGiven = 29 * (randomNumber / 192f)
            };
        }
    }
}