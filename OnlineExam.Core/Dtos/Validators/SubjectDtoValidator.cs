using FluentValidation;
using OnlineExam.Core.Dtos.Subject;

namespace OnlineExam.Core.Dtos.Validators
{
    public class SubjectDtoValidator : AbstractValidator<SubjectViewModel>
    {
        public SubjectDtoValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required");

        }
    }
}
