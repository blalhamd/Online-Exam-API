namespace OnlineExam.Core.Dtos.Choose.Responses
{
    public class ChooseQuestionDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public int ExamId { get; set; }
        public double GradeOfQuestion { get; set; }
        public List<ChoiceDto> Choices { get; set; } = new List<ChoiceDto>();
        public int CorrectAnswerIndex { get; set; }
    }
}
