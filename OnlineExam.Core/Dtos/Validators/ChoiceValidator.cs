using FluentValidation;
using OnlineExam.Core.Dtos.Choose.Requests;

namespace OnlineExam.Core.Dtos.Validators
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
