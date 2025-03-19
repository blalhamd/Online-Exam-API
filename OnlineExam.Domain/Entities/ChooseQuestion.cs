namespace OnlineExam.Domain.Entities
{
    public class ChooseQuestion : Question
    {
        public IList<Choice> Choices { get; set; } = new List<Choice>();

        // Store correct answer as index
        public int CorrectAnswerIndex { get; set; }
    }
}
