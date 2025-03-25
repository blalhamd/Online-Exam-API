using OnlineExam.Core.Dtos.Subject;

namespace OnlineExam.Core.Dtos.Teacher
{
    public class CreateTeacherDto
    {
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public DateTimeOffset HireDate { get; set; }
        public string PhoneNumber { get; set; } = null!;
        public List<SubjectViewModel> Subjects { get; set; } = new();
    }
}
