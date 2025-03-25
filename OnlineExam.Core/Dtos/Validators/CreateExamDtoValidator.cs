using FluentValidation;
using OnlineExam.Core.Dtos.Exam.Request;

namespace OnlineExam.Core.Dtos.Validators
{
    public class CreateExamDtoValidator : AbstractValidator<CreateExamDto>
    {
        public CreateExamDtoValidator()
        {
            // SubjectId must be a positive integer
            RuleFor(x => x.SubjectId)
                .GreaterThan(0)
                .WithMessage("Subject Id must be greater than 0.");

            // TotalGrade must be a positive integer
            RuleFor(x => x.TotalGrade)
                .GreaterThan(0)
                .WithMessage("TotalGrade must be a positive number.");

            // Level must be a positive integer (adjust to your needs)
            RuleFor(x => x.Level)
                .GreaterThan(0)
                .WithMessage("Level must be greater than 0.");

            // Duration - check if non-default TimeOnly
            RuleFor(x => x.Duration)
                .Must(d => d != default)
                .WithMessage("Duration is required and cannot be the default value.");

            // ExamType is an enum, so ensure it's a valid enum value
            RuleFor(x => x.ExamType)
                .IsInEnum()
                .WithMessage("Invalid exam type provided.");

            // Description must not be empty or null
            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage("Description is required.");

            // Validate ChooseQuestions if provided
            RuleForEach(x => x.ChooseQuestions)
                .SetValidator(new ChooseQuestionValidator());

       
        }
    }
}
