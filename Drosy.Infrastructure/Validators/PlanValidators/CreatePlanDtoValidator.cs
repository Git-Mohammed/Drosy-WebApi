using Drosy.Application.UseCases.Plans.DTOs;
using FluentValidation;
using System.Globalization;
using Drosy.Domain.Interfaces.Repository;
using Drosy.Domain.Shared.ErrorComponents.Common;
using Drosy.Domain.Shared.ErrorComponents.Plans;
using Drosy.Domain.Shared.ErrorComponents.Validation;

namespace Drosy.Infrastructure.Validators.PlanValidators;

public class CreatePlanDtoValidator : AbstractValidator<CreatePlanDTo>
{
    private static readonly string[] ValidTypes = ["Individual", "Group"];
    private static readonly string[] ValidStatuses = ["Active", "Inactive"];
    private static readonly string[] ValidDays =
    [
        "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"
    ];

    private readonly IPlanRepository _repository;
    
    public CreatePlanDtoValidator(IPlanRepository  planRepository)
    {
        _repository = planRepository;
        
        RuleFor(x => x.Type)
            .NotEmpty().WithMessage(ValidationErrors.RequiredField.Message)
            .Must(t => ValidTypes.Contains(t, StringComparer.OrdinalIgnoreCase))
            .WithMessage($"{PlanErrors.PlanTypeNotSupported.Message}: {string.Join(", ", ValidTypes)}");

        RuleFor(x => x.Status)
            .NotEmpty().WithMessage(ValidationErrors.RequiredField.Message)
            .Must(s => ValidStatuses.Contains(s, StringComparer.OrdinalIgnoreCase))
            .WithMessage($"Status must be one of: {string.Join(", ", ValidStatuses)}");

        RuleFor(x => x.DaysOfWeek)
            .NotNull().WithMessage(ValidationErrors.RequiredField.Message)
            .Must(list => list.Count > 0).WithMessage(PlanErrors.DaysOfWeekRequired.Message)
            .ForEach(day =>
                day.Must(d => ValidDays.Contains(d, StringComparer.OrdinalIgnoreCase))
                    .WithMessage(d => $"'{d}' is not a valid day."));

        RuleFor(x => x.TotalFees)
            .GreaterThanOrEqualTo(0).WithMessage(PlanErrors.TotalFeesMustBePositive.Message);

        RuleFor(x => x.StartDate)
            .GreaterThanOrEqualTo(DateTime.Today).WithMessage(PlanErrors.InvalidDateRange.Message);

        RuleFor(x => x.Period)
            .GreaterThan(0).WithMessage(PlanErrors.InvalidPeriod.Message);

        RuleFor(x => x.StartSession)
            .LessThan(x => x.EndSession)
            .WithMessage(PlanErrors.SessionTimeConflict.Message);
        
        RuleFor(x => x)
            .MustAsync(async (dto, cancellation) =>
                !await IsExistsPlanAsync(dto.StartSession, dto.EndSession, cancellation))
            .WithMessage(PlanErrors.SessionTimeConflict.Message);
    }

    private async Task<bool> IsExistsPlanAsync(TimeSpan startSession, TimeSpan endSession, CancellationToken cancellationToken)
    {
        return await _repository.ExistsAsync(startSession, endSession, cancellationToken);
    }
}