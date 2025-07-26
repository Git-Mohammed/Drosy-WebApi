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
        public async Task<Result<DataResult<SessionDTO>>> GetSessionsByDate( DateTime date, CancellationToken ct)
        {
            _logger.LogInformation("Fetching sessions for date {Date}", date);
            try
            {
                ct.ThrowIfCancellationRequested();

                var list = (await _sessionRepository.GetSessionsByDateAsync( date, ct)).ToList();

                var dtos = _mapper.Map<List<Session>, List<SessionDTO>>(list);

                var result = new DataResult<SessionDTO> { Data = dtos, TotalRecordsCount = dtos.Count };

                _logger.LogInformation("Fetched {Count} sessions for date {Date}", dtos.Count, date);
                return Result.Success(result);
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("GetSessionsByDate canceled for date {Date}", date);
                return Result.Failure<DataResult<SessionDTO>>(CommonErrors.OperationCancelled);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Error fetching sessions for date {Date}", date); 
                return Result.Failure<DataResult<SessionDTO>>(CommonErrors.Unexpected);
            }
        }

        public async Task<Result<DataResult<SessionDTO>>> GetSessionsInRange( DateTime start, DateTime end, CancellationToken ct)
        {
                _logger.LogInformation("Fetching sessions in range {Start} to {End}", start, end);

            try
            {
                ct.ThrowIfCancellationRequested();
                if (end < start)
                {
                    _logger.LogWarning("Invalid range Start {Start} after End {End}", start, end);
                    return Result.Failure<DataResult<SessionDTO>>(CommonErrors.Invalid);
                }
                var list = (await _sessionRepository.GetSessionsInRangeAsync(start, end, ct)).ToList();
                var dtos = _mapper.Map<List<Session>, List<SessionDTO>>(list);
                var result = new DataResult<SessionDTO> { Data = dtos, TotalRecordsCount = dtos.Count };

                _logger.LogInformation("Fetched {Count} sessions in range {Start} to {End}", dtos.Count, start, end);
                return Result.Success(result);
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("GetSessionsInRange canceled for range {Start} to {End}", start, end);
                return Result.Failure<DataResult<SessionDTO>>(CommonErrors.OperationCancelled);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Error fetching sessions in range {Start} to {End}", start, end);
                return Result.Failure<DataResult<SessionDTO>>(CommonErrors.Unexpected);
            }
        }

        public async Task<Result<DataResult<SessionDTO>>> GetSessionsByWeek(int year, int weekNumber, CancellationToken ct)
        {
                _logger.LogInformation("Fetching sessions for ISO week {Week} of year {Year}", weekNumber, year);

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
                var list = (await _sessionRepository.GetSessionsInRangeAsync( first, last, ct)).ToList();
                var dtos = _mapper.Map<List<Session>, List<SessionDTO>>(list);
                var result = new DataResult<SessionDTO> { Data = dtos, TotalRecordsCount = dtos.Count };

                _logger.LogInformation("Fetched {Count} sessions for week {Week} ({Start} to {End})", dtos.Count, weekNumber, first, last);
                return Result.Success(result);
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("GetSessionsByWeek canceled for year {Year}, week {Week}", year, weekNumber);
                return Result.Failure<DataResult<SessionDTO>>(CommonErrors.OperationCancelled);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Error fetching sessions for week {Week} of year {Year}", weekNumber, year);
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

        public async Task<Result<DataResult<SessionDTO>>> GetSessionsByMonth( int year, int month, CancellationToken ct)
        {
            _logger.LogInformation("Fetching sessions for month {Month} of year {Year}", month, year);
            try
            {
                ct.ThrowIfCancellationRequested();
                if (month < 1 || month > 12)
                {
                    _logger.LogWarning("Invalid month {Month} for year {Year}", month, year);
                    return Result.Failure<DataResult<SessionDTO>>(CommonErrors.Invalid);
                }
                var start = new DateTime(year, month, 1);
                var end = start.AddMonths(1).AddTicks(-1);
                var list = (await _sessionRepository.GetSessionsInRangeAsync(start, end, ct)).ToList();
                var dtos = _mapper.Map<List<Session>, List<SessionDTO>>(list);
                var result = new DataResult<SessionDTO> { Data = dtos, TotalRecordsCount = dtos.Count };
              
                
                _logger.LogInformation("Fetched {Count} sessions for month {Month} ({Start} to {End})", dtos.Count, month, start, end);
                return Result.Success(result);
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("GetSessionsByMonth canceled for year {Year}, month {Month}", year, month);
                return Result.Failure<DataResult<SessionDTO>>(CommonErrors.OperationCancelled);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Error fetching sessions for month {Month} of year {Year}", month, year);
                return Result.Failure<DataResult<SessionDTO>>(CommonErrors.Unexpected);
            }
        }

        public async Task<Result<DataResult<SessionDTO>>> GetSessionsByStatus(SessionStatus status, CancellationToken ct)
        {
            _logger.LogInformation("Fetching sessions with status {Status}", status);

            try
            {
                ct.ThrowIfCancellationRequested();
                if (!Enum.IsDefined(typeof(SessionStatus), status))
                {
                    _logger.LogWarning("Invalid status {Status} provided", status);
                    return Result.Failure<DataResult<SessionDTO>>(CommonErrors.Invalid);
                }
                var list = (await _sessionRepository.GetSessionsByStatusAsync( status, ct)).ToList();
                var dtos = _mapper.Map<List<Session>, List<SessionDTO>>(list);
                var result = new DataResult<SessionDTO> { Data = dtos, TotalRecordsCount = dtos.Count };

                _logger.LogInformation("Fetched {Count} sessions with status {Status}", dtos.Count, status);
                return Result.Success(result);
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("GetSessionsByStatus canceled for status {Status}", status);
                return Result.Failure<DataResult<SessionDTO>>(CommonErrors.OperationCancelled);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Error fetching sessions with status {Status}", status);
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
                session.CreatedAt = dto.NewDate;
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
