
using Drosy.Application.Interfaces.Common;
using Drosy.Application.UseCases.Plans.DTOs;
using Drosy.Application.UseCases.Schedule.DTOs;
using Drosy.Application.UseCases.Schedule.interfaces;
using Drosy.Application.UseCases.Sessions.Services;
using Drosy.Domain.Interfaces.Common.Uow;
using Drosy.Domain.Interfaces.Repository;
using Drosy.Domain.Shared.ApplicationResults;
using Drosy.Domain.Shared.ErrorComponents.Common;

namespace Drosy.Application.UseCases.Schedule.Services
{
    public class CalenderService : ICalenderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISessionRepository _sessionRepository;
        private readonly IPlanRepository _planRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<SessionService> _logger;
        public CalenderService(ISessionRepository sessionRepository, IPlanRepository planRepository,  IUnitOfWork unitOfWork, IMapper mapper, ILogger<SessionService> logger)
        {
            _sessionRepository = sessionRepository;
            _sessionRepository = sessionRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _planRepository = planRepository;
            _logger = logger;
        }
        public async Task<Result<DataResult<CalenderSessionDto>>> GetSessionsCalenderAsync(CancellationToken ct)
        {
            try
            {
                // 1) Load the plans along with its weekly plan days, existing sessions, and enrolled students
                var plans = await _planRepository.GetAllWithDetailsAsync(ct);
                var scheduleEntries = new List<CalenderSessionDto>();

                foreach (var plan in plans)
                {
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
                        var entry = new CalenderSessionDto
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
                }

                // Return the fully built schedule

                var dataResult = new DataResult<CalenderSessionDto> { Data = scheduleEntries, TotalRecordsCount = scheduleEntries.Count };
                return Result.Success(dataResult);
            }
            catch (OperationCanceledException)
            {
                //_logger.LogWarning();
                return Result.Failure<DataResult<CalenderSessionDto>>(CommonErrors.OperationCancelled);
            }
            catch (Exception ex)
            {
                //_logger.LogError();
                return Result.Failure<DataResult<CalenderSessionDto>>(CommonErrors.Unexpected);
            }
        }

        public async Task<Result<DataResult<CalenderSessionDto>>> GetPlanSessionsCalenderAsync(int planId, CancellationToken ct)
        {
            try
            {
                // 1) Load the plan along with its weekly plan days, existing sessions, and enrolled students
                var plan = await _planRepository.GetByIdAsync(planId, ct);

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

                var scheduleEntries = new List<CalenderSessionDto>();

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
                    var entry = new CalenderSessionDto
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

                var dataResult = new DataResult<CalenderSessionDto> { Data = scheduleEntries, TotalRecordsCount = scheduleEntries.Count };
                return Result.Success(dataResult);
            }
            catch (OperationCanceledException)
            {
                //_logger.LogWarning();
                return Result.Failure<DataResult<CalenderSessionDto>>(CommonErrors.OperationCancelled);
            }
            catch (Exception ex)
            {
                //_logger.LogError();
                return Result.Failure<DataResult<CalenderSessionDto>>(CommonErrors.Unexpected);
            }
        }

        public Task<Result<DataResult<CalenderSessionDto>>> GetSessionsCalenderByDateAsync(DateTime date, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task<Result<DataResult<CalenderSessionDto>>> GetSessionsCalenderByMonthAsync(int year, int month, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task<Result<DataResult<CalenderSessionDto>>> GetSessionsCalenderByWeekAsync(int year, int week, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task<Result<DataResult<CalenderSessionDto>>> GetSessionsCalenderInRangeAsync(DateTime start, DateTime end, CancellationToken ct)
        {
            throw new NotImplementedException();
        }
    }
}
