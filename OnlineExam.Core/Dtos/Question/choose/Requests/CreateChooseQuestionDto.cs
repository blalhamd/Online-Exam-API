namespace OnlineExam.Core.Dtos.Question.choose.Requests
{
    public class CreateChooseQuestionDto
    {
        public int ExamId { get; set; }
        public string Title { get; set; } = null!;
        public double GradeOfQuestion { get; set; }
        public List<CreateChoiceDto> Choices { get; set; } = new List<CreateChoiceDto>();
        public int CorrectAnswerIndex { get; set; }
    }
}
