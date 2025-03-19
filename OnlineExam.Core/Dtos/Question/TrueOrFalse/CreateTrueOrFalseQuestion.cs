namespace OnlineExam.Core.Dtos.Question.TrueOrFalse
{
    public class CreateTrueOrFalseQuestion
    {
        public string Title { get; set; } = null!;
        public int ExamId { get; set; }
        public double GradeOfQuestion { get; set; }
        public bool CorrectValue { get; set; }
    }
}
