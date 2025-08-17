using Drosy.Application.Interfaces.Common;
using Drosy.Application.UseCases.Plans.DTOs;
using Drosy.Domain.Entities;
using Drosy.Domain.Interfaces.Repository;
using Drosy.Domain.Shared.ErrorComponents.Plans;
using Drosy.Domain.Shared.ErrorComponents.Validation;
using FluentValidation;

namespace Drosy.Infrastructure.Validators.PlanValidators;

public class CreatePlanDtoValidator : AbstractValidator<CreatePlanDto>
{
    private static readonly string[] ValidTypes = ["Individual", "Group"];
    private static readonly string[] ValidStatuses = ["Active", "Inactive"];

    private readonly IPlanRepository _repository;
    private readonly IMapper _mapper;

    public CreatePlanDtoValidator(IPlanRepository planRepository, IMapper mapper)
    {
        _repository = planRepository;
        _mapper = mapper;

        RuleFor(x => x.Type)
            .NotEmpty().WithMessage(ValidationErrors.RequiredField.Message)
            .Must(t => ValidTypes.Contains(t, StringComparer.OrdinalIgnoreCase))
            .WithMessage($"{PlanErrors.PlanTypeNotSupported.Message}: {string.Join(", ", ValidTypes)}");

        RuleFor(x => x.Status)
            .NotEmpty().WithMessage(ValidationErrors.RequiredField.Message)
            .Must(s => ValidStatuses.Contains(s, StringComparer.OrdinalIgnoreCase))
            .WithMessage($"Status must be one of: {string.Join(", ", ValidStatuses)}");

        RuleFor(x => x.Days)
            .NotNull().WithMessage(ValidationErrors.RequiredField.Message)
            .Must(list => list.Count > 0).WithMessage(PlanErrors.DaysOfWeekRequired.Message);

        // ✅ Validation for each PlanDay item
        RuleForEach(x => x.Days).SetValidator(new PlanDayDtoValidator());

        RuleFor(x => x.TotalFees)
            .GreaterThanOrEqualTo(0).WithMessage(PlanErrors.TotalFeesMustBePositive.Message);

        RuleFor(x => x.StartDate)
            .GreaterThanOrEqualTo(DateTime.Today).WithMessage(PlanErrors.InvalidDateRange.Message);

        RuleFor(x => x.Period)
            .GreaterThan(0).WithMessage(PlanErrors.InvalidPeriod.Message);

        // ✅ Session Conflict Check
        RuleFor(x => x)
            .MustAsync(async (dto, cancellation) =>
                !await IsExistsPlanAsync(dto.Days, cancellation))
            .WithMessage(PlanErrors.SessionTimeConflict.Message);
    }

    private async Task<bool> IsExistsPlanAsync(List<PlanDayDto> daysdto, CancellationToken cancellationToken)
    {
        var days = daysdto.Select(d => _mapper.Map<PlanDayDto,PlanDay>(d)).ToList();
        var isExist = await _repository.ExistsAsync(days, cancellationToken);
        return isExist;
    }
}