using Drosy.Application.UseCases.Plans.DTOs;
using Drosy.Domain.Shared.ResultPattern;

namespace Drosy.Application.UseCases.Plans.Interfaces;

public interface IPlanService
{
    Task<Result<PlanDto>> CreatePlanAsync(CreatePlanDTo newPlan, CancellationToken cancellationToken);
    Task<Result<PlanDto>> GetPlanByIdAsync(int id, CancellationToken cancellationToken);
}