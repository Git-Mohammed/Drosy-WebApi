using Drosy.Application.UseCases.Attendences.DTOs;
using Drosy.Domain.Enums;
using FluentValidation;

namespace Drosy.Infrastructure.Validators.AttendencesValidators
{
    public class UpdateAttendencenDtoValidator : AbstractValidator<UpdateAttendencenDto>
    {
        public UpdateAttendencenDtoValidator()
        {
            RuleFor(x => x.Status)
                .Must(status => Enum.IsDefined(typeof(AttendenceStatus), status))
                .WithMessage("Invalid attendence status value. Allowed: Present, Absent.");

            RuleFor(x => x.Note)
                .NotNull()
                .WithMessage("Note cannot be null.")
                .MaximumLength(500)
                .WithMessage("Note cannot exceed 500 characters.");
        }
    }
}
