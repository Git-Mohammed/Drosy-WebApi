using Drosy.Application.Interfaces.Common;
using Drosy.Application.UseCases.Plans.DTOs;
using Drosy.Application.UseCases.Plans.Interfaces;
using Drosy.Domain.Entities;
using Drosy.Domain.Interfaces.Common.Uow;
using Drosy.Domain.Interfaces.Repository;
using Drosy.Domain.Shared.ApplicationResults;
using Drosy.Domain.Shared.ErrorComponents.Common;
using Drosy.Domain.Shared.ErrorComponents.EFCore;
using Drosy.Domain.Shared.ErrorComponents.Plans;

namespace Drosy.Application.UseCases.Plans.Services;

public class PlanService(
    IPlanRepository planRepository,
    IUnitOfWork unitOfWork,
    ILogger<PlanService> logger,
    IMapper mapper)
    : IPlanService
{
    private readonly IPlanRepository _planRepository = planRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ILogger<PlanService> _logger = logger;
    private readonly IMapper _mapper = mapper;


    public async Task<Result<PlanDto>> CreatePlanAsync(CreatePlanDTo newPlan, CancellationToken cancellationToken)
    {
        var plan = _mapper.Map<CreatePlanDTo, Plan>(newPlan);
        await _planRepository.AddAsync(plan, cancellationToken);
        var result = await _unitOfWork.SaveChangesAsync(cancellationToken);
        if (!result)
        {
            _logger.LogError("Error creating plan", newPlan);
            return Result.Failure<PlanDto>(PlanErrors.PlanSaveFailure);
        }
        var planDto = _mapper.Map<Plan, PlanDto>(plan);
        return Result.Success(planDto);
    }

    public async Task<Result<PlanDto>> GetPlanByIdAsync(int id, CancellationToken cancellationToken)
    {
        var existingPlan = await _planRepository.GetByIdAsync(id, cancellationToken);
        if (existingPlan == null)
            return Result.Failure<PlanDto>(PlanErrors.PlanNotFound);
        var planDto = _mapper.Map<Plan, PlanDto>(existingPlan);
        return Result.Success(planDto);
    }

    public async Task<Result> ExistsAsync(int id, CancellationToken cancellationToken)
    {
        var existingPlan = await _planRepository.ExistsAsync(id, cancellationToken);
        if (!existingPlan)
            return Result.Failure(PlanErrors.PlanNotFound);
        return Result.Success();
    }
}