using OnlineExam.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineExam.Domain.Entities
{
    public class Exam : BaseEntity
    {
        public int SubjectId { get; set; }
        public Subject Subject { get; set; } = null!;
        public int TotalGrade { get; set; }
        public int Level { get; set; }
        public TimeOnly Duration { get; set; }
        public ExamType ExamType { get; set; }
        public string Description { get; set; } = null!;
        public bool Status { get; set; } // (Active) Or (Not Active)
        public List<ChooseQuestion>? ChooseQuestions { get; set; } = new List<ChooseQuestion>();
        public List<StudentExam> StudentExams { get; set; } = new List<StudentExam>(); // Many-to-many with Exams

        [NotMapped]
        public int? NumberOfQuestions => (ChooseQuestions?.Count() ?? 0);

    }
}
