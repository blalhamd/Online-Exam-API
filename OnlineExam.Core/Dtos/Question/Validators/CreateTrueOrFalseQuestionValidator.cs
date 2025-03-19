namespace OnlineExam.Core.ViewModels.Question.Validators
{
    public class CreateTrueOrFalseQuestionValidator : AbstractValidator<CreateTrueOrFalseQuestion>
    {
        public CreateTrueOrFalseQuestionValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.");
            RuleFor(x => x.GradeOfQuestion)
                .GreaterThan(0).WithMessage("Grade must be positive.");
        }
    }
}
