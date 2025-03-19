namespace OnlineExam.Domain.Entities
{
    public class StudentExam : BaseEntity
    {
        public int ExamId { get; set; }
        public Exam Exam { get; set; } = null!;
        public int StudentId { get; set; } 
        public Student Student { get; set; } = null!;
        public bool IsCompleted { get; set; } = false; // Indicates if the student has completed the exam
        public DateTimeOffset? CompletedDate { get; set; } // Date when the student completed the exam
        public DateTimeOffset AssignedDate { get; set; } = DateTimeOffset.Now;
    }
}
