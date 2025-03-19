namespace OnlineExam.Core.Dtos.Question.Validators
{
    public class ChooseQuestionValidator : AbstractValidator<CreateChooseQuestionDto>
    {
        public ChooseQuestionValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.");

            RuleFor(x => x.GradeOfQuestion)
                .GreaterThan(0).WithMessage("Grade must be positive.");

            RuleForEach(x => x.Choices)
                .SetValidator(new ChoiceValidator());

            RuleFor(x => x.CorrectAnswerIndex)
                .GreaterThanOrEqualTo(0).WithMessage("CorrectAnswerIndex must be valid.");
        }
    }
}
