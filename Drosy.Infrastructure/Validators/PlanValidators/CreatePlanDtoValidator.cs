using Drosy.Application.UseCases.Plans.DTOs;
using FluentValidation;
using System.Globalization;
using Drosy.Domain.Interfaces.Repository;

namespace Drosy.Infrastructure.Validators.PlanValidators;

public class CreatePlanDtoValidator : AbstractValidator<CreatePlanDTo>
{
    private static readonly string[] ValidTypes = { "Individual", "Group" };
    private static readonly string[] ValidStatuses = { "Active", "Inactive" };
    private static readonly string[] ValidDays = 
    {
        "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"
    };

    private readonly IPlanRepository _repository;
    
    public CreatePlanDtoValidator(IPlanRepository  planRepository)
    {
        _repository = planRepository;
        
        RuleFor(x => x.Type)
            .NotEmpty().WithMessage("Type is required.")
            .Must(t => ValidTypes.Contains(t, StringComparer.OrdinalIgnoreCase))
            .WithMessage($"Type must be one of: {string.Join(", ", ValidTypes)}");

        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("Status is required.")
            .Must(s => ValidStatuses.Contains(s, StringComparer.OrdinalIgnoreCase))
            .WithMessage($"Status must be one of: {string.Join(", ", ValidStatuses)}");

        RuleFor(x => x.DaysOfWeek)
            .NotNull().WithMessage("DaysOfWeek is required.")
            .Must(list => list.Count > 0).WithMessage("At least one day must be selected.")
            .ForEach(day =>
                day.Must(d => ValidDays.Contains(d, StringComparer.OrdinalIgnoreCase))
                    .WithMessage(d => $"'{d}' is not a valid day."));

        RuleFor(x => x.TotalFees)
            .GreaterThanOrEqualTo(0).WithMessage("Total fees must be >= 0.");

        RuleFor(x => x.StartDate)
            .GreaterThanOrEqualTo(DateTime.Today).WithMessage("Start date cannot be in the past.");

        RuleFor(x => x.Period)
            .GreaterThan(0).WithMessage("Period must be greater than 0.");

        RuleFor(x => x.StartSession)
            .LessThan(x => x.EndSession)
            .WithMessage("Start session time must be earlier than end session time.");
        
        RuleFor(x => x)
            .MustAsync(async (dto, cancellation) =>
                !await IsExistsPlanAsync(dto.StartSession, dto.EndSession, cancellation))
            .WithMessage("A plan already exists for the selected session time.");
    }

    private async Task<bool> IsExistsPlanAsync(TimeSpan startSession, TimeSpan endSession, CancellationToken cancellationToken)
    {
        return await _repository.ExistsAsync(startSession, endSession, cancellationToken);
    }
}