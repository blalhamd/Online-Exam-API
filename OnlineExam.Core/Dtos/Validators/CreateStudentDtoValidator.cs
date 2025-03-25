using FluentValidation;
using OnlineExam.Core.Dtos.Student;

namespace OnlineExam.Core.Dtos.Validators
{
    public class CreateStudentDtoValidator : AbstractValidator<CreateStudentDto>
    {
        public CreateStudentDtoValidator()
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required")
                                .EmailAddress().WithMessage("Email is Invalid");

            RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required")
                                   .MinimumLength(8).WithMessage("Length of Password should't be less than 8 letters.");

            RuleFor(x => x.FullName).NotEmpty().WithMessage("Full Name is required");
            RuleFor(x => x.PhoneNumber).NotEmpty().WithMessage("Phone Number is required");
        }
    }
}
