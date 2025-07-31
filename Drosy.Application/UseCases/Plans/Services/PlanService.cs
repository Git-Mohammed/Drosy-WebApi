using Drosy.Application.Interfaces.Common;
using Drosy.Application.UseCases.Plans.DTOs;
using Drosy.Application.UseCases.Plans.Interfaces;
using Drosy.Application.UseCases.PlanStudents.DTOs;
using Drosy.Application.UseCases.Sessions.DTOs;
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

    public async Task<Result<DataResult<CalenderEntryDto>>> GetPlanSessionsCalenderAsync(int planId, CancellationToken ct)
    {
       try
        {
            // 1) Load the plan along with its weekly plan days, existing sessions, and enrolled students
            var plan = await _planRepository.GetPlanWithDetailsAsync(planId, ct);

            // If no plan is found, bail early with an error
            if (plan == null)
                throw new KeyNotFoundException($"Plan {planId} not found.");

            // 2) Convert bit-flag Days enum into real DayOfWeek values for filtering
            //    e.g. Days.Monday (2) -> log2(2) = 1 -> DayOfWeek.Monday
            var validDaysOfWeek = plan.PlanDays
                .Select(pd => (DayOfWeek)Math.Log2((int)pd.Day))
                .ToHashSet();

            // 3) Determine the start/end dates for iteration
            var startDate = plan.StartDate.Date;
            var endDate = plan.EndDate.Date;

            // 4) Index existing sessions by calendar date for efficient lookup
            var sessionsByDate = plan.Sessions
                .GroupBy(s => s.StartTime.Date)
                .ToDictionary(g => g.Key, g => g.ToList());

            var scheduleEntries = new List<CalenderEntryDto>();

            // 5) Iterate through each day in the plan's duration
            for (var date = startDate; date <= endDate; date = date.AddDays(1))
            {
                // Skip days not defined in the plan's week pattern
                if (!validDaysOfWeek.Contains(date.DayOfWeek))
                    continue;

                // 6) Get the PlanDay object matching this DayOfWeek
                var pd = plan.PlanDays
                    .First(pd2 => (DayOfWeek)Math.Log2((int)pd2.Day) == date.DayOfWeek);

                // Compute the time window for the slot
                var slotStart = date + pd.StartSession;
                var slotEnd = date + pd.EndSession;

                // 7) Lookup any actual session that fully fits within that window
                sessionsByDate.TryGetValue(date, out var sessionsOnDate);
                var session = sessionsOnDate?
                    .FirstOrDefault(s => s.StartTime >= slotStart && s.EndTime <= slotEnd);

                // 8) Project the slot + session data into a DTO
                var entry = new CalenderEntryDto
                {
                    ExcepectedDate = date,
                    PlanId = plan.Id,
                    PlanType = plan.Type.ToString(),
                    PlanStatus = plan.Status.ToString(),

                    // Include all defined weekly slots for reference
                    Days = plan.PlanDays.Select(d => new PlanDayDto
                    {
                        Day = d.Day.ToString(),
                        StartSession = d.StartSession,
                        EndSession = d.EndSession
                    }).ToList(),

                    // If a real session exists, use its ID; otherwise zero indicates an empty slot
                    SessionPeriod = session?.Id ?? 0,

                    // The slot index or session ID can serve as Period
                    Period = session?.Id ?? 0,

                    // List all students for this plan
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

            // Return the fully built schedule

            var dataResult = new DataResult<CalenderEntryDto> { Data = scheduleEntries, TotalRecordsCount = scheduleEntries.Count };
            return Result.Success(dataResult);
        }
        catch (OperationCanceledException)
        {
            //_logger.LogWarning();
            return Result.Failure<DataResult<CalenderEntryDto>>(CommonErrors.OperationCancelled);
        }
        catch (Exception ex)
        {
            //_logger.LogError();
            return Result.Failure<DataResult<CalenderEntryDto>>(CommonErrors.Unexpected);
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