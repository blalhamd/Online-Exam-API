namespace OnlineExam.Core.Dtos.Subject
{
    public class SubjectViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Code { get; set; } // MATH101
        public string? Description { get; set; }
    }
}
