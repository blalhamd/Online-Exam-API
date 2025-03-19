namespace OnlineExam.Core.Dtos.Question.choose.Responses
{
    public class TrueOrFalseQuestionDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public int ExamId { get; set; }
        public double GradeOfQuestion { get; set; }
        public bool CorrectValue { get; set; }
    }
}
