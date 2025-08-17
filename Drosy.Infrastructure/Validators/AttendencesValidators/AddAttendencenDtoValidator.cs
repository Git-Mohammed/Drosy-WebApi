using Drosy.Application.UseCases.Attendences.DTOs;
using Drosy.Domain.Enums;
using Drosy.Domain.Shared.System.Validation.Rules;
using FluentValidation;

namespace Drosy.Infrastructure.Validators.AttendencesValidators
{
    /// <summary>
    /// Validator for <see cref="AddAttendencenDto"/> to ensure the correctness of input data.
    /// </summary>
    public class AddAttendencenDtoValidator : AbstractValidator<AddAttendencenDto>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddAttendencenDtoValidator"/> class.
        /// Defines validation rules for AddAttendencenDto properties.
        /// </summary>
        public AddAttendencenDtoValidator()
        {
            // Validates that StudentId is greater than zero (positive integer).
            RuleFor(x => x.StudentId)
                .GreaterThan(0)
                .WithMessage("Please specify a valid student id.");

            // Validates that Note, if provided, does not exceed 500 characters in length.
            RuleFor(x => x.Note)
                .MaximumLength(TextValidationRules.MaxNotesLength)
                .WithMessage($"Note cannot exceed {TextValidationRules.MaxNotesLength} characters.");

            // Validates that Status is a defined value in the AttendenceStatus enum.
            RuleFor(x => x.Status)
                .Must(status => Enum.IsDefined(typeof(AttendenceStatus), status))
               .WithMessage("The attendance status provided is not valid. Accepted values are: Present or Absent.");
        }
    }
}
