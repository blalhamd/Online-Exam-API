namespace OnlineExam.Domain.Entities
{
    public abstract class Question : BaseEntity
    {
        public string Title { get; set; } = null!;
        public int ExamId { get; set; }
        public Exam Exam { get; set; } = null!;
        public double GradeOfQuestion { get; set; }
    }
}
