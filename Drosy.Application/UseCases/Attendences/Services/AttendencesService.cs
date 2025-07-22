
using Drosy.Application.Interfaces.Common;
using Drosy.Application.UseCases.Attendences.DTOs;
using Drosy.Application.UseCases.Attendences.Interfaces;
using Drosy.Application.UseCases.PlanStudents.DTOs;
using Drosy.Application.UseCases.PlanStudents.Services;
using Drosy.Application.UseCases.Students.Interfaces;
using Drosy.Domain.Entities;
using Drosy.Domain.Enums;
using Drosy.Domain.Interfaces.Common.Uow;
using Drosy.Domain.Interfaces.Repository;
using Drosy.Domain.Shared.ApplicationResults;
using Drosy.Domain.Shared.ErrorComponents;
using Drosy.Domain.Shared.ErrorComponents.Common;
using Drosy.Domain.Shared.ErrorComponents.EFCore;
using System.Numerics;

namespace Drosy.Application.UseCases.Attendences.Services
{

    public class AttendencesService : IAttendencesService
    {
        private readonly IMapper _mapper;
        private readonly IAttendencesRepository _attendencesRepository;
        private readonly IStudentService _studentService;
        //private readonly ISessionService _sessionService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AttendencesService> _logger;

        public AttendencesService(IAttendencesRepository attendencesRepository, IStudentService studentService, ILogger<AttendencesService> logger, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _attendencesRepository = attendencesRepository;
            _studentService = studentService;
            //_sessionService = sessionService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        #region Read
        public async Task<Result<AttendenceDto>> GetByIdAsync(int sessionId, int studentId, CancellationToken ct)
        {
            try
            {
                var attendence = await _attendencesRepository.GetByIdAsync(sessionId, studentId, ct);

                //validation
                //
                var dto = _mapper.Map<Attendence, AttendenceDto>(attendence);
               
                return Result.Success(dto);
            }
            catch (OperationCanceledException)
            {
                return Result.Failure<AttendenceDto>(CommonErrors.OperationCancelled);
            }
            catch (Exception ex)
            {
                return Result.Failure<AttendenceDto>(AppError.Failure);
            }
        }
        public async Task<Result<DataResult<AttendenceDto>>> GetAllForSessionAsync(int sessionId, CancellationToken ct)
        {
           try
            {
                var attendences = (await _attendencesRepository.GetAllForSessionAsync(sessionId, ct)).ToList();

                //validation
                //

                var dtos = _mapper.Map<List<Attendence>, List<AttendenceDto>>(attendences);
                var result = new DataResult<AttendenceDto>
                {
                    Data = dtos,
                    TotalRecordsCount = dtos.Count
                };
                return Result.Success(result);
            }
            catch (OperationCanceledException)
            {
                return Result.Failure<DataResult<AttendenceDto>>(CommonErrors.OperationCancelled);
            }
            catch (Exception ex)
            {
                return Result.Failure<DataResult<AttendenceDto>>(AppError.Failure);
            }
        }

        public async Task<Result<DataResult<AttendenceDto>>> GetAllForStudentAsync(int sessionId, int studentId, CancellationToken ct)
        {
           try
            {
                var attendences = (await _attendencesRepository.GetAllForStudentAsync(sessionId, studentId, ct)).ToList();

                //validation
                //
                var dtos = _mapper.Map<List<Attendence>, List<AttendenceDto>>(attendences);
                var result = new DataResult<AttendenceDto>
                {
                    Data = dtos,
                    TotalRecordsCount = dtos.Count
                };
                return Result.Success(result);
            }
            catch (OperationCanceledException)
            {
                return Result.Failure<DataResult<AttendenceDto>>(CommonErrors.OperationCancelled);
            }
            catch (Exception ex)
            {
                return Result.Failure<DataResult<AttendenceDto>>(AppError.Failure);
            }
        }

        public async Task<Result<DataResult<AttendenceDto>>> GetAllForSessionByStatusAsync(int sessionId, AttendenceStatus status, CancellationToken ct)
        {
         try
            {
                var attendences = (await _attendencesRepository.GetAllForStudentByStatusAsync(sessionId, status, ct)).ToList();
                //validation
                //

                var dtos = _mapper.Map<List<Attendence>, List<AttendenceDto>>(attendences);
                var result = new DataResult<AttendenceDto>
                {
                    Data = dtos,
                    TotalRecordsCount = dtos.Count
                };
                return Result.Success(result);
            }
            catch (OperationCanceledException)
            {
                return Result.Failure<DataResult<AttendenceDto>>(CommonErrors.OperationCancelled);
            }
            catch (Exception ex)
            {
                return Result.Failure<DataResult<AttendenceDto>>(AppError.Failure);
            }
        }


        #endregion

        #region Write
        public async Task<Result<AttendenceDto>> AddAsync(int sessionId, AddAttendencenDto dto, CancellationToken ct)
        {
            _logger.LogInformation("Starting AddAttendenceAsync for SessionId={SessionId}, StudentId={StudentId}", sessionId, dto.StudentId);
            try
            {
                ct.ThrowIfCancellationRequested();

                #region Validations
                var existing = await _attendencesRepository.ExistsAsync(sessionId, dto.StudentId, ct);
                if (existing)
                {
                    _logger.LogWarning("Attendence already exists for SessionId={SessionId}, StudentId={StudentId}", sessionId, dto.StudentId);
                    return Result.Failure<AttendenceDto>(CommonErrors.Conflict);
                }

                #endregion

                var attendence = _mapper.Map<AddAttendencenDto, Attendence>(dto);

                await _attendencesRepository.AddAsync(attendence, ct);
                bool isSaved = await _unitOfWork.SaveChangesAsync(ct);

                if (!isSaved)
                {
                    _logger.LogError("Failed to save changes when adding Student {StudentId} Attendence to Session {SessionId}", dto.StudentId, sessionId);
                    return Result.Failure<AttendenceDto>(EfCoreErrors.CanNotSaveChanges);
                }

                var attendenceDtp = _mapper.Map<Attendence, AttendenceDto>(attendence);

                _logger.LogInformation("Successfully added Student Attendence to Session {SessionId}", dto.StudentId, sessionId);
                return Result.Success(attendenceDtp);

            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Operation canceled in AddAttendenceAsync for Session {SessionId}", sessionId);
                return Result.Failure<AttendenceDto>(CommonErrors.OperationCancelled);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Unexpected error in AddAttendenceAsync for SessionId={SessionId}, StudentId={StudentId}", sessionId, dto.StudentId);
                return Result.Failure<AttendenceDto>(AppError.Failure);
            }
        }

        public async Task<Result<DataResult<AttendenceDto>>> AddRangeAsync(int sessionId, IEnumerable<AddAttendencenDto> dtos, CancellationToken ct)
        {
            _logger.LogInformation("Starting AddRangeOfAttendenceAsync for SessionId={SessionId}, StudentCount={Count}", sessionId, dtos?.Count() ?? 0);
            try
            {
                ct.ThrowIfCancellationRequested();

                #region Validations
                // Get all already existing attendances for this session and students
                var studentIds = dtos.Select(d => d.StudentId).Distinct().ToList();
                var existingAttendences = await _attendencesRepository
                    .GetAllForStudentBySessionAsync(sessionId, studentIds, ct); 

                var existingStudentIds = existingAttendences.Select(a => a.StudentId).ToHashSet();

                // Filter out students who already have attendances in this session
                var newDtos = dtos.Where(dto => !existingStudentIds.Contains(dto.StudentId)).ToList();

                // 
                if (!newDtos.Any())
                {
                    _logger.LogWarning("All provided students already have attendances in session {SessionId}", sessionId);
                    return Result.Failure<DataResult<AttendenceDto>>(CommonErrors.Conflict);
                }
                #endregion

                var attendences = _mapper.Map<IEnumerable<AddAttendencenDto>, List<Attendence>>(newDtos);
                attendences.ForEach(a => a.SessionId = sessionId);

                await _attendencesRepository.AddRangeAsync(attendences, ct);
                bool isSaved = await _unitOfWork.SaveChangesAsync(ct);

                if (!isSaved)
                {
                    _logger.LogError("Failed to save batch add for SessionId={SessionId}", sessionId);
                    return Result.Failure<DataResult<AttendenceDto>>(EfCoreErrors.CanNotSaveChanges);
                }

                var attendenceDtos = _mapper.Map<List<Attendence>, List<AttendenceDto>>(attendences);
                var dataResult = new DataResult<AttendenceDto>
                {
                    Data = attendenceDtos ?? Enumerable.Empty<AttendenceDto>(),
                    TotalRecordsCount = attendenceDtos!.Count
                };


                _logger.LogInformation("Successfully added {Count} Student Attendences to Session {SessionId}", dataResult.TotalRecordsCount, sessionId);
                return Result.Success(dataResult);

            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Operation canceled in AddRangeOfAttendenceAsync for Session {SessionId}", sessionId);
                return Result.Failure<DataResult<AttendenceDto>>(CommonErrors.OperationCancelled);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Unexpected error in AddRangeOfAttendenceAsync for SessionId={SessionId}", sessionId);
                return Result.Failure<DataResult<AttendenceDto>>(AppError.Failure);
            }
        }

        public async Task<Result<AttendenceDto>> UpdateAsync(int sessionId, int studentId, UpdateAttendencenDto dto, CancellationToken ct)
        {
            _logger.LogInformation("Starting UpdateAttendenceAsync for SessionId={SessionId}, StudentId={StudentId}", sessionId, studentId);

            try
            {
                ct.ThrowIfCancellationRequested();

                #region Validations
                // 1) Does the record exist?
                var attendence = await _attendencesRepository.GetByIdAsync(sessionId, studentId, ct);
                if (attendence == null)
                {
                    _logger.LogWarning("Attendence not found for SessionId={SessionId}, StudentId={StudentId}", sessionId, studentId);
                    return Result.Failure<AttendenceDto>(CommonErrors.NotFound);
                }
                #endregion

                _mapper.Map(dto, attendence);


                bool isSaved = await _unitOfWork.SaveChangesAsync(ct);
                if (!isSaved)
                {
                    _logger.LogError("Failed to save changes when updating Attendence for SessionId={SessionId}, StudentId={StudentId}", sessionId, studentId);
                    return Result.Failure<AttendenceDto>(EfCoreErrors.CanNotSaveChanges);
                }

                var updatedDto = _mapper.Map<Attendence, AttendenceDto>(attendence);

                _logger.LogInformation("Successfully updated Attendence for SessionId={SessionId}, StudentId={StudentId}", sessionId, studentId);

                return Result.Success(updatedDto);
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Operation canceled in UpdateAttendenceAsync for SessionId={SessionId}, StudentId={StudentId}", sessionId, studentId);
                return Result.Failure<AttendenceDto>(CommonErrors.OperationCancelled);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Unexpected error in UpdateAttendenceAsync for SessionId={SessionId}, StudentId={StudentId}", sessionId, studentId);
                return Result.Failure<AttendenceDto>(AppError.Failure);
            }
        }

        public Task<Result> DeleteAsync(int sessionId, int studentId, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        
        #endregion
    }

}

