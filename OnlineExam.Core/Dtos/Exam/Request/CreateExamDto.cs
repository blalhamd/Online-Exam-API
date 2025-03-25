using OnlineExam.Core.Dtos.Choose.Requests;
using OnlineExam.Domain.Enums;

namespace OnlineExam.Core.Dtos.Exam.Request
{
    public class CreateExamDto
    {
        public int SubjectId { get; set; }
        public int TotalGrade { get; set; }
        public int Level { get; set; }
        public TimeOnly Duration { get; set; }
        public ExamType ExamType { get; set; }
        public string Description { get; set; } = null!;
        public bool Status { get; set; } // (Active) Or (Not Active)
        public List<CreateChooseQuestionDto> ChooseQuestions { get; set; } = new List<CreateChooseQuestionDto>();
    }
}
