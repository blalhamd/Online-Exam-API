namespace OnlineExam.Domain.Entities
{
    public class UserAnswer : BaseEntity
    {
        public int ExamAttemptId { get; set; }
        public ExamAttempt ExamAttempt { get; set; } = null!;

        public int ChooseQuestionId { get; set; }
        public ChooseQuestion ChooseQuestion { get; set; } = null!;

        public int? SelectedChoiceId { get; set; } // For MCQ questions

        public double Score { get; set; } = 0; // Calculated based on correctness
    }
}
