using System.Security.AccessControl;
using Drosy.Application.Interfaces.Common;
using Drosy.Application.UseCases.Plans.DTOs;
using Drosy.Application.UseCases.Plans.Interfaces;
using Drosy.Domain.Entities;
using Drosy.Domain.Enums;
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


    public async Task<Result<PlanDto>> CreatePlanAsync(CreatePlanDto newPlan, CancellationToken cancellationToken)
    {
        var plan = _mapper.Map<CreatePlanDto, Plan>(newPlan);
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

    public async Task<Result<DataResult<PlanDto>>> GetAllPlansAsync(CancellationToken cancellationToken)
    {
        var existingPlans = await _planRepository.GetAllAsync(cancellationToken);
        if (!existingPlans.Any())
            return Result.Failure<DataResult<PlanDto>>(PlanErrors.PlanNotFound);
        var existingPlansDto = _mapper.Map<List<Plan>, List<PlanDto>>(existingPlans.ToList());
        return Result.Success(new DataResult<PlanDto>
        {
            Data = existingPlansDto,
            TotalRecordsCount = existingPlansDto.Count,
        });
    }

    public async Task<Result> ExistsAsync(int id, CancellationToken cancellationToken)
    {
        var existingPlan = await _planRepository.ExistsAsync(id, cancellationToken);
        if (!existingPlan)
            return Result.Failure(PlanErrors.PlanNotFound);
        return Result.Success();
    }

    public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var existingPlan = await _planRepository.GetByIdAsync(id, cancellationToken);
        if (existingPlan == null)
            return Result.Failure(PlanErrors.PlanNotFound);
        
        if(existingPlan.Status == PlanStatus.Active)
            return Result.Failure(PlanErrors.PlanCannotBeDeletedWithStudents);
        await _planRepository.DeleteAsync(existingPlan, cancellationToken);
        var result = await _unitOfWork.SaveChangesAsync(cancellationToken);
        if (!result)
        {
            _logger.LogError("Error deleting plan", existingPlan);
            return Result.Failure(PlanErrors.PlanDeleteFailure);
        }
        return Result.Success();
    }

    public async Task<Result> UpdateStatusAsync(int id, UpdatePlanStatusDto status, CancellationToken cancellationToken)
    {
        var existingPlan = await _planRepository.GetByIdAsync(id, cancellationToken);
        if (existingPlan == null)
            return Result.Failure(PlanErrors.PlanNotFound);
        var planstatus = _mapper.Map<UpdatePlanStatusDto, PlanStatus>(status);
        if (existingPlan.Status == planstatus)
            return Result.Success();
        existingPlan.Status = planstatus;
        await _planRepository.UpdateAsync(existingPlan, cancellationToken);
        var result = await _unitOfWork.SaveChangesAsync(cancellationToken);
        if (!result)
        {
            _logger.LogError("Error updating plan", existingPlan);
            return Result.Failure(PlanErrors.PlanSaveFailure);
        }
        return Result.Success();
    }
}