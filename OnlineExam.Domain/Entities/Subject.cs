namespace OnlineExam.Domain.Entities
{
    public class Subject : BaseEntity
    {
        public string Name { get; set; } = null!;
        public string? Code { get; set; } // MATH101
        public string? Description { get; set; }
        public List<TeacherSubject> TeacherSubjects { get; set; } = new();
    }
}
