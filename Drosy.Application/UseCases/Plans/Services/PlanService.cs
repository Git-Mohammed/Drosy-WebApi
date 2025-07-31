using Drosy.Application.Interfaces.Common;
using Drosy.Application.UseCases.Plans.DTOs;
using Drosy.Application.UseCases.Plans.Interfaces;
using Drosy.Application.UseCases.PlanStudents.DTOs;
using Drosy.Application.UseCases.Schedule.DTOs;
using Drosy.Domain.Entities;
using Drosy.Domain.Enums;
using Drosy.Domain.Interfaces.Common.Uow;
using Drosy.Domain.Interfaces.Repository;
using Drosy.Domain.Shared.ApplicationResults;
using Drosy.Domain.Shared.ErrorComponents.Common;
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


    #region Read

    
    #region abstraction methods
    public async Task<Result<List<Plan>>> GetAllPlansWithDetailsAsync(CancellationToken ct)
    {
        try
        {
            var plans = await _planRepository.GetAllWithDetailsAsync(ct);
            return Result.Success(plans.ToList());
        }
        catch (OperationCanceledException)
        {
            return Result.Failure<List<Plan>>(CommonErrors.OperationCancelled);
        }
        catch (Exception ex)
        {
            return Result.Failure<List<Plan>>(CommonErrors.Unexpected);
        }
    }

    public async Task<Result<List<Plan>>> GetPlansWithDetailsByStatusAsync(PlanStatus status, CancellationToken ct)
    {
        try
        {
            var plans = await _planRepository.GetAllByStatusAsync(status, ct);
            return Result.Success(plans.ToList());
        }
        catch (OperationCanceledException)
        {
            return Result.Failure<List<Plan>>(CommonErrors.OperationCancelled);
        }
        catch (Exception ex)
        {
            return Result.Failure<List<Plan>>(CommonErrors.Unexpected);
        }
    }

    #endregion


    public async Task<Result<PlanDto>> GetPlanByIdAsync(int id, CancellationToken cancellationToken)
    {
        var existingPlan = await _planRepository.GetByIdAsync(id, cancellationToken);
        if (existingPlan == null)
            return Result.Failure<PlanDto>(PlanErrors.PlanNotFound);
        var planDto = _mapper.Map<Plan, PlanDto>(existingPlan);
        return Result.Success(planDto);
    }

    #region Calender
    public async Task<Result<DataResult<CalenderSessionDto>>> GetPlanSessionsCalenderAsync(int planId, CancellationToken ct)
    {
        try
        {
            // Load the plan with details
            var plan = await _planRepository.GetByIdAsync(planId, ct);
            if (plan == null)
                return Result.Failure<DataResult<CalenderSessionDto>>(PlanErrors.PlanNotFound);

            // Generate sessions using the new method
            var scheduleEntries = await GenerateSessionsForPlanAsync(plan, null, null, ct);

            var dataResult = new DataResult<CalenderSessionDto> { Data = scheduleEntries, TotalRecordsCount = scheduleEntries.Count };
            return Result.Success(dataResult);
        }
        catch (OperationCanceledException)
        {
            return Result.Failure<DataResult<CalenderSessionDto>>(CommonErrors.OperationCancelled);
        }
        catch (Exception ex)
        {
            return Result.Failure<DataResult<CalenderSessionDto>>(CommonErrors.Unexpected);
        }
    }

    public async Task<Result<DataResult<CalenderSessionDto>>> GetPlanSessionsCalenderByDateAsync(int planId, DateTime date, CancellationToken ct)
    {
       try
        {
            var plan = await _planRepository.GetByIdAsync(planId, ct);
            if (plan == null)
                return Result.Failure<DataResult<CalenderSessionDto>>(PlanErrors.PlanNotFound);

            var sessions = await GenerateSessionsForPlanAsync(plan, date, date, ct);
            return Result.Success(new DataResult<CalenderSessionDto> { Data = sessions, TotalRecordsCount = sessions.Count });
        }
      catch (OperationCanceledException)
        {
            return Result.Failure<DataResult<CalenderSessionDto>>(CommonErrors.OperationCancelled);
        }
        catch (Exception ex)
        {
            return Result.Failure<DataResult<CalenderSessionDto>>(CommonErrors.Unexpected);
        }
    }

    public async Task<Result<DataResult<CalenderSessionDto>>> GetPlanSessionsCalenderByRangeAsync(int planId, DateTime start, DateTime end, CancellationToken ct)
    {
      try
        {
            if (start > end)
                return Result.Failure<DataResult<CalenderSessionDto>>(PlanErrors.ConstraintViolation);

            var plan = await _planRepository.GetByIdAsync(planId, ct);
            if (plan == null)
                return Result.Failure<DataResult<CalenderSessionDto>>(PlanErrors.PlanNotFound);

            var sessions = await GenerateSessionsForPlanAsync(plan, start, end, ct);
            return Result.Success(new DataResult<CalenderSessionDto> { Data = sessions, TotalRecordsCount = sessions.Count });
        }
        catch (OperationCanceledException)
        {
            return Result.Failure<DataResult<CalenderSessionDto>>(CommonErrors.OperationCancelled);
        }
        catch (Exception ex)
        {
            return Result.Failure<DataResult<CalenderSessionDto>>(CommonErrors.Unexpected);
        }
    }

    public async Task<Result<DataResult<CalenderSessionDto>>> GetPlanSessionsCalenderByStatusAsync(int planId, PlanStatus status, CancellationToken ct)
    {
      try
        {
              var plan = await _planRepository.GetByIdAsync(planId, ct);
        if (plan == null)
            return Result.Failure<DataResult<CalenderSessionDto>>(PlanErrors.PlanNotFound);

        if (plan.Status != status)
            return Result.Success(new DataResult<CalenderSessionDto> { Data = new List<CalenderSessionDto>(), TotalRecordsCount = 0 });

        var sessions = await GenerateSessionsForPlanAsync(plan, null, null, ct);
        return Result.Success(new DataResult<CalenderSessionDto> { Data = sessions, TotalRecordsCount = sessions.Count });
        }
        catch (OperationCanceledException)
        {
            return Result.Failure<DataResult<CalenderSessionDto>>(CommonErrors.OperationCancelled);
        }
        catch (Exception ex)
        {
            return Result.Failure<DataResult<CalenderSessionDto>>(CommonErrors.Unexpected);
        }
    }

    public async Task<Result<DataResult<CalenderSessionDto>>> GetPlanSessionsCalenderByWeekAsync(int planId, int year, int week, CancellationToken ct)
    {
     try
        {
            var startOfWeek = GetStartOfWeek(year, week);
            var endOfWeek = startOfWeek.AddDays(6);
            return await GetPlanSessionsCalenderByRangeAsync(planId, startOfWeek, endOfWeek, ct);
        }
        catch (OperationCanceledException)
        {
            return Result.Failure<DataResult<CalenderSessionDto>>(CommonErrors.OperationCancelled);
        }
        catch (Exception ex)
        {
            return Result.Failure<DataResult<CalenderSessionDto>>(CommonErrors.Unexpected);
        }
    }

    public async Task<Result<DataResult<CalenderSessionDto>>> GetPlanSessionsCalenderByMonthAsync(int planId, int year, int month, CancellationToken ct)
    {
       try
        {
            var start = new DateTime(year, month, 1);
            var end = start.AddMonths(1).AddDays(-1);
            return await GetPlanSessionsCalenderByRangeAsync(planId, start, end, ct);
        }
        catch (OperationCanceledException)
        {
            return Result.Failure<DataResult<CalenderSessionDto>>(CommonErrors.OperationCancelled);
        }
        catch (Exception ex)
        {
            return Result.Failure<DataResult<CalenderSessionDto>>(CommonErrors.Unexpected);
        }
    }

    public async Task<List<CalenderSessionDto>> GenerateSessionsForPlanAsync(Plan plan, DateTime? startFilter, DateTime? endFilter, CancellationToken ct)
    {
        // plan

        var scheduleEntries = new List<CalenderSessionDto>();

        var validDaysOfWeek = plan.PlanDays
            .Select(pd => (DayOfWeek)Math.Log2((int)pd.Day))
            .ToHashSet();

        var startDate = startFilter?.Date ?? plan.StartDate.Date;
        var endDate = endFilter?.Date ?? plan.EndDate.Date;

        var sessionsByDate = plan.Sessions
            .Where(s => !startFilter.HasValue || s.StartTime.Date >= startFilter.Value.Date)
            .Where(s => !endFilter.HasValue || s.StartTime.Date <= endFilter.Value.Date)
            .GroupBy(s => s.StartTime.Date)
            .ToDictionary(g => g.Key, g => g.ToList());

        for (var date = startDate; date <= endDate; date = date.AddDays(1))
        {
            ct.ThrowIfCancellationRequested(); 

            if (!validDaysOfWeek.Contains(date.DayOfWeek))
                continue;

            var pd = plan.PlanDays
                .First(pd2 => (DayOfWeek)Math.Log2((int)pd2.Day) == date.DayOfWeek);

            var slotStart = date + pd.StartSession;
            var slotEnd = date + pd.EndSession;

            sessionsByDate.TryGetValue(date, out var sessionsOnDate);
            var session = sessionsOnDate?
                .FirstOrDefault(s => s.StartTime >= slotStart && s.EndTime <= slotEnd);

            var entry = new CalenderSessionDto
            {
                ExcepectedDate = date,
                PlanId = plan.Id,
                PlanType = plan.Type.ToString(),
                PlanStatus = plan.Status.ToString(),
                Days = plan.PlanDays.Select(d => new PlanDayDto
                {
                    Day = d.Day.ToString(),
                    StartSession = d.StartSession,
                    EndSession = d.EndSession
                }).ToList(),
                SessionPeriod = session?.Id ?? 0,
                Period = session?.Id ?? 0,
                Students = plan.Students.Select(ps => new CalenderPlanStudentDto
                {
                    StudentId = ps.StudentId,
                    Notes = ps.Notes,
                    Fee = ps.Fee,
                    CreatedAt = ps.CreatedAt,
                    FullName = string.Join(" ", new[] {
                        ps.Student.FirstName,
                        ps.Student.SecondName,
                        ps.Student.ThirdName,
                        ps.Student.LastName
                    }.Where(name => !string.IsNullOrWhiteSpace(name))),
                    Address = ps.Student.Address,
                    PhoneNumber = ps.Student.PhoneNumber
                }).ToList()
            };

            scheduleEntries.Add(entry);
        }

        return await Task.FromResult(scheduleEntries); // Wrap synchronous result in Task
    }

    private DateTime GetStartOfWeek(int year, int week)
    {
        var jan4 = new DateTime(year, 1, 4);
        var startOfYear = jan4.AddDays(-((int)jan4.DayOfWeek + 6) % 7);
        return startOfYear.AddDays((week - 1) * 7);
    }
    #endregion

    public async Task<Result> ExistsAsync(int id, CancellationToken cancellationToken)
    {
        var existingPlan = await _planRepository.ExistsAsync(id, cancellationToken);
        if (!existingPlan)
            return Result.Failure(PlanErrors.PlanNotFound);
        return Result.Success();
    }
 
    public Task<Result<DataResult<PlanDto>>> GetAllAsync(CancellationToken cancellationToken)
        => BuildDataResultAsync(
            () => _planRepository.GetAllAsync(cancellationToken),
            cancellationToken);

    public Task<Result<DataResult<PlanDto>>> GetPlansByDate(DateTime date, CancellationToken cancellationToken)
        => BuildDataResultAsync(
            () => _planRepository.GetAllByDateAsync(date, cancellationToken),
            cancellationToken);

    public Task<Result<DataResult<PlanDto>>> GetPlansInRange(DateTime start, DateTime end, CancellationToken cancellationToken)
        => BuildDataResultAsync(
            () => _planRepository.GetAllInRangeAsync(start, end, cancellationToken),
            cancellationToken);

    public Task<Result<DataResult<PlanDto>>> GetPlansByStatus(PlanStatus status, CancellationToken cancellationToken)
        => BuildDataResultAsync(
            () => _planRepository.GetAllByStatusAsync(status, cancellationToken),
            cancellationToken);

    public Task<Result<DataResult<PlanDto>>> GetPlansByType(PlanTypes type, CancellationToken cancellationToken)
        => BuildDataResultAsync(
            () => _planRepository.GetAllByTypeAsync(type, cancellationToken),
            cancellationToken);

    public Task<Result<DataResult<PlanDto>>> GetPlansByWeek(int year, int week, CancellationToken cancellationToken)
        => BuildDataResultAsync(
            () => _planRepository.GetAllByWeekAsync(year, week, cancellationToken),
            cancellationToken);

    public Task<Result<DataResult<PlanDto>>> GetPlansByMonth(int year, int month, CancellationToken cancellationToken)
        => BuildDataResultAsync(
            () => _planRepository.GetAllByMonthAsync(year, month, cancellationToken),
            cancellationToken);
   
    private async Task<Result<DataResult<PlanDto>>> BuildDataResultAsync( Func<Task<IEnumerable<Plan>>> fetchFunc, CancellationToken cancellationToken)
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