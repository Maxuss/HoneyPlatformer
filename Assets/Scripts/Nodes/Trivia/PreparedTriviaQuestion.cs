namespace Nodes.Trivia
{
    public struct PreparedTriviaQuestion: ITriviaQuestion
    {
        public float TimeGiven;
        public string Question;
        public string[] Answers;
        public int CorrectAnswer;
        public float Reward;
        
        public PreparedTriviaQuestion Prepare()
        {
            return this;
        }
    }
}