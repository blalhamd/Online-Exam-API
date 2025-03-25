namespace OnlineExam.Domain.Entities
{
    public class ChooseQuestion : BaseEntity
    {
        public string Title { get; set; } = null!;
        public int ExamId { get; set; }
        public Exam Exam { get; set; } = null!;
        public double GradeOfQuestion { get; set; }
        public IList<Choice> Choices { get; set; } = new List<Choice>();

        // Store correct answer as index
        public int CorrectAnswerIndex { get; set; }
    }
}
