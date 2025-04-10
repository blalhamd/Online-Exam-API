namespace OnlineExam.Core.Dtos.UserAnswer
{
    public class UserAnswerDto
    {
        public int QuestionId { get; set; }
        public int? SelectedChoiceId { get; set; } // For MCQ questions
    }
}
