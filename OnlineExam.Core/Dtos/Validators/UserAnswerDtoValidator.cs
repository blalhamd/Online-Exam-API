using FluentValidation;
using OnlineExam.Core.Dtos.UserAnswer;

namespace OnlineExam.Core.Dtos.Validators
{
    public class UserAnswerDtoValidator : AbstractValidator<UserAnswerDto>
    {
        public UserAnswerDtoValidator()
        {
            RuleFor(x => x.QuestionId).NotEmpty().WithMessage("Question Number is required");
        }
    }
}
