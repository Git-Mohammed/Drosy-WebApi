using Drosy.Application.Interfaces.Common;
using Drosy.Application.UseCases.Plans.DTOs;
using Drosy.Application.UseCases.Plans.Interfaces;
using Drosy.Application.UseCases.Sessions.DTOs;
using Drosy.Domain.Entities;
using Drosy.Domain.Enums;
using Drosy.Domain.Interfaces.Common.Uow;
using Drosy.Domain.Interfaces.Repository;
using Drosy.Domain.Shared.ApplicationResults;
using Drosy.Domain.Shared.ErrorComponents.Common;
using Drosy.Domain.Shared.ErrorComponents.EFCore;
using Drosy.Domain.Shared.ErrorComponents.Plans;
using static System.Runtime.InteropServices.JavaScript.JSType;

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


    #region Read

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
  
    private async Task<Result<DataResult<PlanDto>>> BuildDataResultAsync(
        Func<Task<IEnumerable<Plan>>> fetchFunc,
        CancellationToken cancellationToken)
    {
        try
        {
            var list = (await fetchFunc()).ToList();
            var dtos = _mapper.Map<List<Plan>, List<PlanDto>>(list);

            var dataResult = new DataResult<PlanDto> { Data = dtos, TotalRecordsCount = dtos.Count };

            return Result.Success(dataResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, "Error building data result");
            return Result.Failure<DataResult<PlanDto>>(CommonErrors.Unexpected);
        }
    }

    public Task<Result<DataResult<PlanDto>>> GetAllAsync(CancellationToken cancellationToken)
        => BuildDataResultAsync(
            () => _planRepository.GetAllAsync(cancellationToken),
            cancellationToken);

    public Task<Result<DataResult<PlanDto>>> GetPlansByDate(DateTime date, CancellationToken cancellationToken)
        => BuildDataResultAsync(
            () => _planRepository.GetByDateAsync(date, cancellationToken),
            cancellationToken);

    public Task<Result<DataResult<PlanDto>>> GetPlansInRange(DateTime start, DateTime end, CancellationToken cancellationToken)
        => BuildDataResultAsync(
            () => _planRepository.GetInRangeAsync(start, end, cancellationToken),
            cancellationToken);

    public Task<Result<DataResult<PlanDto>>> GetPlansByStatus(PlanStatus status, CancellationToken cancellationToken)
        => BuildDataResultAsync(
            () => _planRepository.GetByStatusAsync(status, cancellationToken),
            cancellationToken);

    public Task<Result<DataResult<PlanDto>>> GetPlansByType(PlanTypes type, CancellationToken cancellationToken)
        => BuildDataResultAsync(
            () => _planRepository.GetByTypeAsync(type, cancellationToken),
            cancellationToken);

    public Task<Result<DataResult<PlanDto>>> GetPlansByWeek(int year, int week, CancellationToken cancellationToken)
        => BuildDataResultAsync(
            () => _planRepository.GetByWeekAsync(year, week, cancellationToken),
            cancellationToken);

    public Task<Result<DataResult<PlanDto>>> GetPlansByMonth(int year, int month, CancellationToken cancellationToken)
        => BuildDataResultAsync(
            () => _planRepository.GetByMonthAsync(year, month, cancellationToken),
            cancellationToken);
    #endregion


    #region Write


    public async Task<Result<PlanDto>> CreatePlanAsync(CreatePlanDto newPlan, CancellationToken cancellationToken)
    {
        //_logger.LogInformation();

        try
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

            //_logger.LogInformation();
            return Result.Success(planDto);
        }
        catch (OperationCanceledException)
        {
            //_logger.LogWarning();
            return Result.Failure<PlanDto>(CommonErrors.OperationCancelled);
        }
        catch (Exception ex)
        {
            //_logger.LogError();
            return Result.Failure<PlanDto>(CommonErrors.Unexpected);
        }

    }


    #endregion

}