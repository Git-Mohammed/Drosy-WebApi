using Drosy.Application.UseCases.Students.DTOs;
using FluentValidation;

namespace Drosy.Infrastructure.Validators.StudentValidator
{
    public class AddStudentValidator : AbstractValidator<AddStudentDTO>
    {

        // TODO:
        // 1- Validation Duplication in PhoneNumber and EmergencyNumber
        public AddStudentValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required.")
                .MaximumLength(50).WithMessage("First name must not exceed 50 characters.");

            RuleFor(x => x.SecondName)
                .NotEmpty().WithMessage("Second name is required.")
                .MaximumLength(50).WithMessage("Second name must not exceed 50 characters.");

            RuleFor(x => x.ThirdName)
                .MaximumLength(50).WithMessage("Third name must not exceed 50 characters.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required.")
                .MaximumLength(50).WithMessage("Last name must not exceed 50 characters.");

            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("Address is required.")
                .MaximumLength(200).WithMessage("Address must not exceed 200 characters.");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required.")
                .Matches(@"^\+?\d{7,15}$").WithMessage("Phone number is not valid.");

            RuleFor(x => x.EmergencyNumber)
                .NotEmpty().WithMessage("Emergency number is required.")
                .Matches(@"^\+?\d{7,15}$").WithMessage("Emergency number is not valid.");

            RuleFor(x => x.GradeId)
                .GreaterThan(0).WithMessage("GradeId must be greater than 0.");

            RuleFor(x => x.CityId)
                .GreaterThan(0).WithMessage("CityId must be greater than 0.");

            // UserId is optional, so no validation required
            RuleFor(x => x.UserId).GreaterThan(0).WithMessage("UserId must be null or grater than 0.")
                .When(x => x.UserId.HasValue, ApplyConditionTo.CurrentValidator);
        }
    }
}