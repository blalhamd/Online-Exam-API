using FluentValidation;
using OnlineExam.Core.Dtos.Teacher;

namespace OnlineExam.Core.Dtos.Validators
{
    public class CreateTeacherDtoValidator : AbstractValidator<CreateTeacherDto>
    {
        public CreateTeacherDtoValidator()
        {

            RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required")
                                .EmailAddress().WithMessage("Email is Invalid");

            RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required")
                                   .MinimumLength(8).WithMessage("Length of Password should't be less than 8 letters.");

            RuleFor(x => x.FullName).NotEmpty().WithMessage("Full Name is required");

            RuleFor(x => x.HireDate).NotNull().WithMessage("Hire Date is required");
            RuleFor(x => x.PhoneNumber).NotNull().WithMessage("Phone Number is required");
        }
    }
}
