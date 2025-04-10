namespace OnlineExam.Core.Dtos.Score
{
    public class AnsweredQuestionViewModel
    {
        public int QuestionId { get; set; }
        public int ExamId { get; set; }
        public string Title { get; set; } = null!;
        public double GradeOfQuestion { get; set; }
        public bool IsCorrect { get; set; }
    }
}
