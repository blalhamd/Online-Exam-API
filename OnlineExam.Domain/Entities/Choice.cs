namespace OnlineExam.Domain.Entities
{
    public class Choice : BaseEntity
    {
        public string Text { get; set; } = null!;
        public int ChooseQuestionId { get; set; }
        public ChooseQuestion ChooseQuestion { get; set; } = null!;
    }
}
