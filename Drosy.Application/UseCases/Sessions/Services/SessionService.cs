using Drosy.Application.Interfaces.Common;
using Drosy.Application.UseCases.Plans.DTOs;
using Drosy.Application.UseCases.Plans.Interfaces;
using Drosy.Application.UseCases.Plans.Services;
using Drosy.Application.UseCases.PlanStudents.DTOs;
using Drosy.Application.UseCases.Sessions.DTOs;
using Drosy.Application.UseCases.Sessions.Interfaces;
using Drosy.Domain.Entities;
using Drosy.Domain.Enums;
using Drosy.Domain.Interfaces.Common.Uow;
using Drosy.Domain.Interfaces.Repository;
using Drosy.Domain.Shared.ApplicationResults;
using Drosy.Domain.Shared.ErrorComponents.Common;
using Drosy.Domain.Shared.ErrorComponents.EFCore;
using Drosy.Domain.Shared.ErrorComponents.Sesstions;
using Drosy.Domain.Shared.System.CalandeHelper;
using System.Globalization;
using static System.Runtime.InteropServices.JavaScript.JSType;



namespace Drosy.Application.UseCases.Sessions.Services
{
    public class SessionService : ISessionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISessionRepository _sessionRepository;
        private readonly IPlanService _planService;
        private readonly IMapper _mapper;
        private readonly ILogger<SessionService> _logger;
        public SessionService(ISessionRepository sessionRepository,IPlanService planService, IUnitOfWork unitOfWork, IMapper mapper, ILogger<SessionService> logger)
        {
            _sessionRepository = sessionRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _planService = planService;
            _logger = logger;
        }


        #region Read
        public async Task<Result<SessionDTO>> GetByIdAsync(int id, CancellationToken ct)
        {
            try
            {
                var session = await _sessionRepository.GetByIdAsync(id, ct);

                if (session == null)
                {
                    return Result.Failure<SessionDTO>(SessionErrors.SessionNotFound); // or define SessionErrors.SessionNotFound if more precise
                }

                var sessionDTO = _mapper.Map<Session, SessionDTO>(session);
                return Result.Success(sessionDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Error retrieving session with ID {SessionId}", id);
                return Result.Failure<SessionDTO>(CommonErrors.Unexpected);
            }
        }
       
        #region Non-plan methods
        public Task<Result<DataResult<SessionDTO>>> GetAllAsync(CancellationToken ct)
            => BuildDataResultAsync(() => _sessionRepository.GetAllAsync(ct), ct);

        public Task<Result<DataResult<SessionDTO>>> GetSessionsByDate(DateTime date, CancellationToken ct)
            => BuildDataResultAsync(() => _sessionRepository.GetAllByDateAsync(date, ct), ct);

        public Task<Result<DataResult<SessionDTO>>> GetSessionsInRange(DateTime start, DateTime end, CancellationToken ct)
            => BuildDataResultAsync(() => _sessionRepository.GetAllInRangeAsync(start, end, ct), ct);

        public Task<Result<DataResult<SessionDTO>>> GetSessionsByWeek(int year, int weekNumber, CancellationToken ct)
            => BuildDataResultAsync(() => _sessionRepository.GetAllByWeekAsync(year, weekNumber, ct), ct);

        public Task<Result<DataResult<SessionDTO>>> GetSessionsByMonth(int year, int month, CancellationToken ct)
            => BuildDataResultAsync(() => _sessionRepository.GetAllByMonthAsync(year, month, ct), ct);

        public Task<Result<DataResult<SessionDTO>>> GetSessionsByStatus(SessionStatus status, CancellationToken ct)
            => BuildDataResultAsync(() => _sessionRepository.GetAllByStatusAsync(status, ct), ct);


        #region Calender
        public async Task<Result<DataResult<CalenderSessionDto>>> GetSessionsCalenderAsync(CancellationToken ct)
        {
            try
            {
                // Fetch all plans via PlanService
                var plansResult = await _planService.GetAllPlansWithDetailsAsync(ct);
                if (!plansResult.IsSuccess)
                    return Result.Failure<DataResult<CalenderSessionDto>>(plansResult.Error);

                var scheduleEntries = new List<CalenderSessionDto>();

                foreach (var plan in plansResult.Value)
                {
                    scheduleEntries.AddRange(await _planService.GenerateSessionsForPlanAsync(plan, null, null, ct));
                }

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

        public async Task<Result<DataResult<CalenderSessionDto>>> GetSessionsCalenderByDate(DateTime date, CancellationToken ct)
        {
            try
            {
                var plansResult = await _planService.GetAllPlansWithDetailsAsync(ct);
                if (!plansResult.IsSuccess)
                    return Result.Failure<DataResult<CalenderSessionDto>>(plansResult.Error);

                var scheduleEntries = new List<CalenderSessionDto>();
                date = date.Date;

                foreach (var plan in plansResult.Value)
                {
                    if (date < plan.StartDate.Date || date > plan.EndDate.Date)
                        continue;

                    var validDaysOfWeek = plan.PlanDays
                        .Select(pd => (DayOfWeek)Math.Log2((int)pd.Day))
                        .ToHashSet();

                    if (!validDaysOfWeek.Contains(date.DayOfWeek))
                        continue;

                    scheduleEntries.AddRange(await _planService.GenerateSessionsForPlanAsync(plan, date, date, ct));
                }

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

        public async Task<Result<DataResult<CalenderSessionDto>>> GetSessionsCalenderInRange(DateTime start, DateTime end, CancellationToken ct)
        {
            try
            {
                var plansResult = await _planService.GetAllPlansWithDetailsAsync(ct);
                if (!plansResult.IsSuccess)
                    return Result.Failure<DataResult<CalenderSessionDto>>(plansResult.Error);

                start = start.Date;
                end = end.Date;

                if (start > end)
                    return Result.Failure<DataResult<CalenderSessionDto>>(SessionErrors.ExpectedDateInThePast);

                var scheduleEntries = new List<CalenderSessionDto>();

                foreach (var plan in plansResult.Value)
                {
                    var planStart = plan.StartDate.Date;
                    var planEnd = plan.EndDate.Date;

                    var rangeStart = start > planStart ? start : planStart;
                    var rangeEnd = end < planEnd ? end : planEnd;

                    if (rangeStart > rangeEnd)
                        continue;

                    scheduleEntries.AddRange(await _planService.GenerateSessionsForPlanAsync(plan, rangeStart, rangeEnd, ct));
                }

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

        public async Task<Result<DataResult<CalenderSessionDto>>> GetSessionsCalenderByWeek(int year, int weekNumber, CancellationToken ct)
        {
            try
            {
                var jan1 = new DateTime(year, 1, 1);
                var daysOffset = DayOfWeek.Monday - jan1.DayOfWeek;
                if (daysOffset < 0) daysOffset += 7;
                var firstMonday = jan1.AddDays(daysOffset);
                var start = firstMonday.AddDays((weekNumber - 1) * 7);
                var end = start.AddDays(6);

                return await GetSessionsCalenderInRange(start, end, ct);
            }
            catch (Exception ex)
            {
                return Result.Failure<DataResult<CalenderSessionDto>>(CommonErrors.Unexpected);
            }
        }

        public async Task<Result<DataResult<CalenderSessionDto>>> GetSessionsCalenderByMonth(int year, int month, CancellationToken ct)
        {
            try
            {
                var start = new DateTime(year, month, 1);
                var end = start.AddMonths(1).AddDays(-1);
                return await GetSessionsCalenderInRange(start, end, ct);
            }
            catch (Exception ex)
            {
                return Result.Failure<DataResult<CalenderSessionDto>>(CommonErrors.Unexpected);
            }
        }

        public async Task<Result<DataResult<CalenderSessionDto>>> GetSessionsCalenderByStatus(PlanStatus status, CancellationToken ct)
        {
            try
            {
                var plansResult = await _planService.GetPlansWithDetailsByStatusAsync(status, ct);
                if (!plansResult.IsSuccess)
                    return Result.Failure<DataResult<CalenderSessionDto>>(plansResult.Error);

                var scheduleEntries = new List<CalenderSessionDto>();

                foreach (var plan in plansResult.Value)
                {
                    scheduleEntries.AddRange(await _planService.GenerateSessionsForPlanAsync(plan, null, null, ct));
                }

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

        #endregion


        #endregion

        #region Plan-scoped methods
        public Task<Result<DataResult<SessionDTO>>> GetSessionsByPlan(int planId, CancellationToken ct)
            => BuildDataResultAsync(() => _sessionRepository.GetAllByPlanAsync(planId, ct), ct);

        public Task<Result<DataResult<SessionDTO>>> GetSessionsByDate(int planId, DateTime date, CancellationToken ct)
            => BuildDataResultAsync(() => _sessionRepository.GetAllByPlanAndDateAsync(planId, date, ct), ct);

        public Task<Result<DataResult<SessionDTO>>> GetSessionsInRange(int planId, DateTime start, DateTime end, CancellationToken ct)
            => BuildDataResultAsync(() => _sessionRepository.GetAllByPlanAndRangeAsync(planId, start, end, ct), ct);

        public Task<Result<DataResult<SessionDTO>>> GetSessionsByWeek(int planId, int year, int weekNumber, CancellationToken ct)
            => BuildDataResultAsync(() => _sessionRepository.GetAllByPlanAndWeekAsync(planId, year, weekNumber, ct), ct);

        public Task<Result<DataResult<SessionDTO>>> GetSessionsByMonth(int planId, int year, int month, CancellationToken ct)
            => BuildDataResultAsync(() => _sessionRepository.GetAllByPlanAndMonthAsync(planId, year, month, ct), ct);

        public Task<Result<DataResult<SessionDTO>>> GetSessionsByStatus(int planId, SessionStatus status, CancellationToken ct)
            => BuildDataResultAsync(() => _sessionRepository.GetAllByPlanAndStatusAsync(planId, status, ct), ct);



        private async Task<Result<DataResult<SessionDTO>>> BuildDataResultAsync(
            Func<Task<IEnumerable<Session>>> fetchFunc,
            CancellationToken ct)
        {
            try
            {
                ct.ThrowIfCancellationRequested();
                var list = (await fetchFunc()).ToList();
                var dtos = _mapper.Map<List<Session>, List<SessionDTO>>(list);

                var dataResult = new DataResult<SessionDTO> { Data = dtos, TotalRecordsCount = dtos.Count };

                return Result.Success(dataResult);
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Operation canceled in BuildDataResultAsync");
                return Result.Failure<DataResult<SessionDTO>>(CommonErrors.OperationCancelled);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Error building session data result");
                return Result.Failure<DataResult<SessionDTO>>(CommonErrors.Unexpected);
            }
        }

        #endregion


        #endregion

        #region Write
        #endregion

        public async Task<Result<SessionDTO>> CreateAsync(CreateSessionDTO sessionDTO, CancellationToken ct)
        {
            try
            {
                // 🔒 Basic validation
                if (string.IsNullOrWhiteSpace(sessionDTO.Title))
                    return Result.Failure<SessionDTO>(SessionErrors.TitleRequired);

                if (sessionDTO.StartTime >= sessionDTO.EndTime)
                    return Result.Failure<SessionDTO>(SessionErrors.StartAfterEnd);

                if (sessionDTO.StartTime.Date != sessionDTO.ExcepectedDate.Date ||
                    sessionDTO.EndTime.Date != sessionDTO.ExcepectedDate.Date)
                    return Result.Failure<SessionDTO>(SessionErrors.OutsideExpectedDate);

                // 🕒 Check for overlapping sessions using optimized existence query
                bool hasOverlap = await _sessionRepository
                    .ExistsAsync(sessionDTO.ExcepectedDate, sessionDTO.StartTime, sessionDTO.EndTime, ct);

                if (hasOverlap)
                    return Result.Failure<SessionDTO>(SessionErrors.TimeOverlap);
                // 🧱 Mapping using IMapper
                var newSession = _mapper.Map<CreateSessionDTO, Session>(sessionDTO);

                await _sessionRepository.AddAsync(newSession, ct);

                // 💾 Commit using IUnitOfWork
                var saved = await _unitOfWork.SaveChangesAsync(ct);
                if (!saved)
                    return Result.Failure<SessionDTO>(SessionErrors.InvalidTimeRange); // fallback error

                // 📦 Return result
                var resultDTO = _mapper.Map<Session, SessionDTO>(newSession);
                return Result.Success(resultDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Error adding session");
                return Result.Failure<SessionDTO>(CommonErrors.Unexpected);
            }
        }

        public async Task<Result<SessionDTO>> RescheduleAsync(int sessionId, RescheduleSessionDTO dto, CancellationToken ct)
        {
            try
            {
                // 🔍 Retrieve session
                var session = await _sessionRepository.GetByIdAsync(sessionId, ct);
                if (session == null)
                    return Result.Failure<SessionDTO>(SessionErrors.SessionNotFound);

                // ✅ Validate time range
                if (dto.NewStartTime >= dto.NewEndTime)
                    return Result.Failure<SessionDTO>(SessionErrors.StartAfterEnd);

                if (dto.NewStartTime.Date != dto.NewDate.Date || dto.NewEndTime.Date != dto.NewDate.Date)
                    return Result.Failure<SessionDTO>(SessionErrors.OutsideExpectedDate);

                // ⛔ Check for overlap
                bool overlapExists = await _sessionRepository.ExistsAsync(sessionId,
                    dto.NewDate, dto.NewStartTime, dto.NewEndTime, ct);

                if (overlapExists)
                    return Result.Failure<SessionDTO>(SessionErrors.TimeOverlap);

                // 🕒 Update values
                session.CreatedAt = dto.NewDate;
                session.StartTime = dto.NewStartTime;
                session.EndTime = dto.NewEndTime;

                await _sessionRepository.UpdateAsync(session, ct);
                var saved = await _unitOfWork.SaveChangesAsync(ct);

                if (!saved)
                    return Result.Failure<SessionDTO>(EfCoreErrors.CanNotSaveChanges);

                // 📝 Log reason
                _logger.LogInformation("Session {SessionId} rescheduled to {NewDate} {NewStart}–{NewEnd}", sessionId, dto.NewDate, dto.NewStartTime, dto.NewEndTime);

                // 📦 Return DTO
                var resultDTO = _mapper.Map<Session, SessionDTO>(session);
                return Result.Success(resultDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Error rescheduling session {SessionId}", sessionId);
                return Result.Failure<SessionDTO>(CommonErrors.Unexpected);
            }
        }
    }
}
