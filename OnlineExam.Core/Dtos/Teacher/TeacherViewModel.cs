namespace OnlineExam.Core.Dtos.Teacher
{
    public class TeacherViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public DateTimeOffset HireDate { get; set; }
        public string PhoneNumber { get; set; } = null!;
    }
}
