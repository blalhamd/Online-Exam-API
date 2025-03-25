using OnlineExam.Domain.Entities.Identity;

namespace OnlineExam.Domain.Entities
{
    public class Teacher : BaseEntity
    {
        public DateTimeOffset HireDate { get; set; } // Date the teacher was hired
        public string UserId { get; set; } = null!;
        public AppUser User { get; set; } = null!;
        public List<TeacherSubject> TeacherSubjects { get; set; } = new();
    }
}
