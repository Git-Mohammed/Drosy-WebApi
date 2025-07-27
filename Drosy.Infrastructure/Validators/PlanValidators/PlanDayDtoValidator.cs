using Drosy.Application.UseCases.Plans.DTOs;
using Drosy.Domain.Shared.ErrorComponents.Plans;
using Drosy.Domain.Shared.ErrorComponents.Validation;
using FluentValidation;

namespace Drosy.Infrastructure.Validators.PlanValidators;
public class PlanDayDtoValidator : AbstractValidator<PlanDayDto>
{
    private static readonly string[] ValidDays =
    [
        "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"
    ];

    public PlanDayDtoValidator()
    {
        RuleFor(x => x.Day)
            .NotEmpty().WithMessage(ValidationErrors.RequiredField.Message)
            .Must(d => ValidDays.Contains(d, StringComparer.OrdinalIgnoreCase))
            .WithMessage(d => $"'{d.Day}' is not a valid day.");

        RuleFor(x => x.StartSession)
            .LessThan(x => x.EndSession)
            .WithMessage(PlanErrors.SessionTimeConflict.Message);
    }
}
