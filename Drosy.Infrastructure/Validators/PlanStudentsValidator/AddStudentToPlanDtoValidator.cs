using Drosy.Application.UseCases.PlanStudents.DTOs;
using FluentValidation;
namespace Drosy.Infrastructure.Validators.PlanStudentsValidator
{
    public class AddStudentToPlanDtoValidator : AbstractValidator<AddStudentToPlanDto>
    {
        public AddStudentToPlanDtoValidator()
        {
            RuleFor(x => x.StudentId)
                .GreaterThan(0)
                .WithMessage("Please specify a valid student.");

            RuleFor(x => x.Notes)
                .MaximumLength(500)
                .WithMessage("Notes cannot exceed 500 characters.");
        }
    }
}
