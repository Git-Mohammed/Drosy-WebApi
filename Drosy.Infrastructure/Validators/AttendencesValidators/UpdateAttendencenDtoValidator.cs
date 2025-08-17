using Drosy.Application.UseCases.Attendences.DTOs;
using Drosy.Domain.Enums;
using Drosy.Domain.Shared.System.Validation.Rules;
using FluentValidation;

namespace Drosy.Infrastructure.Validators.AttendencesValidators
{
    /// <summary>
    /// Validator for the <see cref="UpdateAttendencenDto"/> class.
    /// Validates the attendance update data ensuring the status is valid and note length constraints.
    /// </summary>
    public class UpdateAttendencenDtoValidator : AbstractValidator<UpdateAttendencenDto>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateAttendencenDtoValidator"/> class 
        /// and configures validation rules.
        /// </summary>
        public UpdateAttendencenDtoValidator()
        {
            // Validate that the Status is a defined value in the AttendenceStatus enum
            RuleFor(x => x.Status)
                .Must(status => Enum.IsDefined(typeof(AttendenceStatus), status))
                .WithMessage("The attendance status provided is not valid. Accepted values are: Present or Absent.");

            // Validate that the Note property does not exceed 500 characters
            RuleFor(x => x.Note)
                 .MaximumLength(TextValidationRules.MaxNotesLength)
                 .WithMessage($"Note cannot exceed {TextValidationRules.MaxNotesLength} characters.");
        }
    }
}
