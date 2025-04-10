namespace OnlineExam.Core.Dtos.Score
{
    public class ScoreViewModel
    {
        public double Score { get; set; }
        public double ScoreInPercentage { get; set; }
        public double TotalGrade { get; set; }

        public int NumberQuestions { get; set; }
        public int NumberWrongQuestions { get; set; }
        public int NumberCorrectQuestions { get; set; }
        public bool Status => ScoreInPercentage > 60;
        public List<AnsweredQuestionViewModel> AnsweredQuestions { get; set; } = new();
    }
}
