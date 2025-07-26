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

        public Task<Result<DataResult<SessionDTO>>> GetSessionsByWeek(int planId, int year, int weekNumber, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task<Result<DataResult<SessionDTO>>> GetSessionsByMonth(int planId, int year, int month, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task<Result<DataResult<SessionDTO>>> GetSessionsByStatus(int planId, SessionStatus status, CancellationToken c)
        {
            throw new NotImplementedException();
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
