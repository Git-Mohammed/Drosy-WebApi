using Drosy.Application.UseCases.PlanStudents.DTOs;
using FluentValidation;

namespace Drosy.Infrastructure.Validators.PlanStudentsValidator
{
    /// <summary>
    /// Validates instances of <see cref="AddStudentToPlanDto"/>.
    /// </summary>
    /// <remarks>
    /// Ensures that:
    /// <list type="bullet">
    ///   <item><description><c>StudentId</c> is greater than zero.</description></item>
    ///   <item><description><c>Notes</c> does not exceed 500 characters.</description></item>
    ///   <item><description><c>Fee</c> is zero or a positive amount.</description></item>
    /// </list>
    /// </remarks>
    public class AddStudentToPlanDtoValidator : AbstractValidator<AddStudentToPlanDto>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddStudentToPlanDtoValidator"/> class
        /// and defines validation rules for <see cref="AddStudentToPlanDto"/>.
        /// </summary>
        public AddStudentToPlanDtoValidator()
        {
            // Rule: StudentId must be a positive integer
            RuleFor(x => x.StudentId)
                .GreaterThan(0)
                .WithMessage("Please specify a valid student.");

            // Rule: Notes length must not exceed 500 characters
            RuleFor(x => x.Notes)
                .MaximumLength(500)
                .WithMessage("Notes cannot exceed 500 characters.");

            // Rule: Fee must be zero or positive
            RuleFor(x => x.Fee)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Fee must be zero or a positive amount.");
        }
    }
}
