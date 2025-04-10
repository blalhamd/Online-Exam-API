using OnlineExam.Domain.Entities.Identity;

namespace OnlineExam.Domain.Entities
{
    public class ExamAttempt : BaseEntity
    {
        public string UserId { get; set; } = null!;
        public AppUser User { get; set; } = null!;

        public int ExamId { get; set; }
        public Exam Exam { get; set; } = null!;

        public DateTimeOffset StartTime { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? EndTime { get; set; } // Null if exam is still in progress.

        public double Score { get; set; } = 0; // Total score after submission
        public bool IsSubmitted { get; set; } = false; // True when user submits the exam

        public ICollection<UserAnswer> UserAnswers { get; set; } = new List<UserAnswer>();
    }
}
