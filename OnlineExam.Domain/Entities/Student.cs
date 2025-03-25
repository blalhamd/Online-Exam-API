using OnlineExam.Domain.Entities.Identity;

namespace OnlineExam.Domain.Entities
{
    public class Student : BaseEntity
    {
        public string UserId { get; set; } = null!; // Foreign key to AppUser
        public AppUser User { get; set; } = null!; // Navigation property
        public List<StudentExam> StudentExams { get; set; } = new List<StudentExam>(); // Many-to-many with Exams
    }
}
