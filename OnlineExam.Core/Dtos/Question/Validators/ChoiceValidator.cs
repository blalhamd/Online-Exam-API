namespace OnlineExam.Core.Dtos.Question.Validators
{
    public class ChoiceValidator : AbstractValidator<CreateChoiceDto>
    {
        public ChoiceValidator()
        {
            RuleFor(x => x.Text)
                .NotEmpty().WithMessage("Choice text cannot be empty.");
        }
    }
}
