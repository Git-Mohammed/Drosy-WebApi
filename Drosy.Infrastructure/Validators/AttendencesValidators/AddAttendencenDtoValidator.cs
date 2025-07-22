using Drosy.Application.UseCases.Attendences.DTOs;
using Drosy.Domain.Enums;
using FluentValidation;

namespace Drosy.Infrastructure.Validators.AttendencesValidators
{
    public class AddAttendencenDtoValidator : AbstractValidator<AddAttendencenDto>
    {
      
        public AddAttendencenDtoValidator()
        {
            RuleFor(x => x.StudentId)
                .GreaterThan(0)
                .WithMessage("Please specify a valid student id.");

            RuleFor(x => x.Note)
                 .NotNull()
                .WithMessage("Note cannot be null.")
                .MaximumLength(500)
                .WithMessage("Note cannot exceed 500 characters.");

            RuleFor(x => x.Status)
                   .Must(status => Enum.IsDefined(typeof(AttendenceStatus), status))
                   .WithMessage("Invalid attendence status value. Allowed: Present, Absent.");
        }
    }
}
