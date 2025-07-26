using Drosy.Application.Interfaces.Common;
using Drosy.Application.UseCases.Plans.DTOs;
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
using System.Globalization;



namespace Drosy.Application.UseCases.Sessions.Services
{
    public class SessionService : ISessionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISessionRepository _sessionRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<SessionService> _logger;
        public SessionService(ISessionRepository sessionRepository, IUnitOfWork unitOfWork, IMapper mapper, ILogger<SessionService> logger)
        {
            _sessionRepository = sessionRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;

            _logger = logger;
        }


        #region Read
        public async Task<Result<SessionDTO>> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            try
            {
                var session = await _sessionRepository.GetByIdAsync(id, cancellationToken);

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
        public async Task<Result<DataResult<SessionDTO>>> GetSessionsByDate(int planId, DateTime date, CancellationToken ct)
        {
            _logger.LogInformation("Fetching sessions for PlanId={PlanId} on {Date}", planId, date);
            try
            {
                ct.ThrowIfCancellationRequested();

                var list = (await _sessionRepository.GetSessionsByDateAsync(planId, date, ct)).ToList();

                var dtos = _mapper.Map<List<Session>, List<SessionDTO>>(list);

                var result = new DataResult<SessionDTO> { Data = dtos, TotalRecordsCount = dtos.Count };
                return Result.Success(result);
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("GetSessionsByDate canceled for PlanId={PlanId}", planId);
                return Result.Failure<DataResult<SessionDTO>>(CommonErrors.OperationCancelled);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Error in GetSessionsByDate for PlanId={PlanId}", planId);
                return Result.Failure<DataResult<SessionDTO>>(CommonErrors.Unexpected);
            }
        }

        public async Task<Result<DataResult<SessionDTO>>> GetSessionsInRange(int planId, DateTime start, DateTime end, CancellationToken ct)
        {
            _logger.LogInformation("Fetching sessions for PlanId={PlanId} from {Start} to {End}", planId, start, end);
            try
            {
                ct.ThrowIfCancellationRequested();
                if (end < start)
                {
                    _logger.LogWarning("Invalid range Start {Start} after End {End}", start, end);
                    return Result.Failure<DataResult<SessionDTO>>(CommonErrors.Invalid);
                }
                var list = (await _sessionRepository.GetSessionsInRangeAsync(planId, start, end, ct)).ToList();
                var dtos = _mapper.Map<List<Session>, List<SessionDTO>>(list);
                var result = new DataResult<SessionDTO> { Data = dtos, TotalRecordsCount = dtos.Count };
                return Result.Success(result);
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("GetSessionsInRange canceled for PlanId={PlanId}", planId);
                return Result.Failure<DataResult<SessionDTO>>(CommonErrors.OperationCancelled);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Error in GetSessionsInRange for PlanId={PlanId}", planId);
                return Result.Failure<DataResult<SessionDTO>>(CommonErrors.Unexpected);
            }
        }

        public async Task<Result<DataResult<SessionDTO>>> GetSessionsByWeek(int planId, int year, int weekNumber, CancellationToken ct)
        {
            _logger.LogInformation("Fetching sessions for PlanId={PlanId}, Year={Year}, Week={Week}", planId, year, weekNumber);
            try
            {
                ct.ThrowIfCancellationRequested();
                if (weekNumber < 1 || weekNumber > 53)
                {
                    _logger.LogWarning("Invalid week number {Week} for year {Year}", weekNumber, year);
                    return Result.Failure<DataResult<SessionDTO>>(CommonErrors.Invalid);
                }
                var first = GetFirstDateOfIsoWeek(year, weekNumber);
                var last = first.AddDays(6);
                var list = (await _sessionRepository.GetSessionsInRangeAsync(planId, first, last, ct)).ToList();
                var dtos = _mapper.Map<List<Session>, List<SessionDTO>>(list);
                var result = new DataResult<SessionDTO> { Data = dtos, TotalRecordsCount = dtos.Count };
                return Result.Success(result);
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("GetSessionsByWeek canceled for PlanId={PlanId}", planId);
                return Result.Failure<DataResult<SessionDTO>>(CommonErrors.OperationCancelled);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Error in GetSessionsByWeek for PlanId={PlanId}", planId);
                return Result.Failure<DataResult<SessionDTO>>(CommonErrors.Unexpected);
            }
        }
        // Helper to compute Monday of ISO week
        private static DateTime GetFirstDateOfIsoWeek(int year, int weekNumber)
        {
            // ISO 8601: week 1 is the week with the year's first Thursday
            var jan4 = new DateTime(year, 1, 4);
            int dayOfWeek = ((int)jan4.DayOfWeek + 6) % 7; // Monday=0
            var monday = jan4.AddDays(-dayOfWeek);
            return monday.AddDays((weekNumber - 1) * 7);
        }

        public Task<Result<DataResult<SessionDTO>>> GetSessionsByMonth(int planId, int year, int month, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public async Task<Result<DataResult<SessionDTO>>> GetSessionsByStatus(int planId, SessionStatus status, CancellationToken ct)
        {
            _logger.LogInformation("Fetching sessions for PlanId={PlanId} with Status={Status}", planId, status);
            try
            {
                ct.ThrowIfCancellationRequested();
                if (!Enum.IsDefined(typeof(SessionStatus), status))
                {
                    _logger.LogWarning("Invalid status {Status} provided", status);
                    return Result.Failure<DataResult<SessionDTO>>(CommonErrors.Invalid);
                }
                var list = (await _sessionRepository.GetSessionsByStatusAsync(planId, status, ct)).ToList();
                var dtos = _mapper.Map<List<Session>, List<SessionDTO>>(list);
                var result = new DataResult<SessionDTO> { Data = dtos, TotalRecordsCount = dtos.Count };
                return Result.Success(result);
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("GetSessionsByStatus canceled for PlanId={PlanId}", planId);
                return Result.Failure<DataResult<SessionDTO>>(CommonErrors.OperationCancelled);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Error in GetSessionsByStatus for PlanId={PlanId}", planId);
                return Result.Failure<DataResult<SessionDTO>>(CommonErrors.Unexpected);
            }
        }
        #endregion

        #region Write
        #endregion

        public async Task<Result<SessionDTO>> CreateAsync(CreateSessionDTO sessionDTO, CancellationToken cancellationToken)
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
                    .ExistsAsync(sessionDTO.ExcepectedDate, sessionDTO.StartTime, sessionDTO.EndTime, cancellationToken);

                if (hasOverlap)
                    return Result.Failure<SessionDTO>(SessionErrors.TimeOverlap);
                // 🧱 Mapping using IMapper
                var newSession = _mapper.Map<CreateSessionDTO, Session>(sessionDTO);

                await _sessionRepository.AddAsync(newSession, cancellationToken);

                // 💾 Commit using IUnitOfWork
                var saved = await _unitOfWork.SaveChangesAsync(cancellationToken);
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

        public async Task<Result<SessionDTO>> RescheduleAsync(int sessionId, RescheduleSessionDTO dto, CancellationToken cancellationToken)
        {
            try
            {
                // 🔍 Retrieve session
                var session = await _sessionRepository.GetByIdAsync(sessionId, cancellationToken);
                if (session == null)
                    return Result.Failure<SessionDTO>(SessionErrors.SessionNotFound);

                // ✅ Validate time range
                if (dto.NewStartTime >= dto.NewEndTime)
                    return Result.Failure<SessionDTO>(SessionErrors.StartAfterEnd);

                if (dto.NewStartTime.Date != dto.NewDate.Date || dto.NewEndTime.Date != dto.NewDate.Date)
                    return Result.Failure<SessionDTO>(SessionErrors.OutsideExpectedDate);

                // ⛔ Check for overlap
                bool overlapExists = await _sessionRepository.ExistsAsync(sessionId,
                    dto.NewDate, dto.NewStartTime, dto.NewEndTime, cancellationToken);

                if (overlapExists)
                    return Result.Failure<SessionDTO>(SessionErrors.TimeOverlap);

                // 🕒 Update values
                session.ExcepectedDate = dto.NewDate;
                session.StartTime = dto.NewStartTime;
                session.EndTime = dto.NewEndTime;

                await _sessionRepository.UpdateAsync(session, cancellationToken);
                var saved = await _unitOfWork.SaveChangesAsync(cancellationToken);

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
