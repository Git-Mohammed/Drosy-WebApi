using Drosy.Application.Interfaces.Common;
using Drosy.Application.UseCases.Plans.DTOs;
using Drosy.Application.UseCases.Sessions.DTOs;
using Drosy.Application.UseCases.Sessions.Interfaces;
using Drosy.Domain.Entities;
using Drosy.Domain.Interfaces.Common.Uow;
using Drosy.Domain.Interfaces.Repository;
using Drosy.Domain.Shared.ApplicationResults;
using Drosy.Domain.Shared.ErrorComponents.Common;
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

        public async Task<Result<SessionDTO>> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            try
            {
                var session = await _sessionRepository.GetByIdAsync(id, cancellationToken);

                if (session == null)
                {
                    return Result.Failure<SessionDTO>(SessionErrors.PlanNotFound); // or define SessionErrors.SessionNotFound if more precise
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
        public async Task<Result<SessionDTO>> AddAsync(AddSessionDTO sessionDTO, CancellationToken cancellationToken)
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
                    .SessionExistsAsync(sessionDTO.ExcepectedDate, sessionDTO.StartTime, sessionDTO.EndTime, cancellationToken);

                if (hasOverlap)
                    return Result.Failure<SessionDTO>(SessionErrors.TimeOverlap);
                // 🧱 Mapping using IMapper
                var newSession = _mapper.Map<AddSessionDTO, Session>(sessionDTO);

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

    }
}
