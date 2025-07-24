using Drosy.Application.Interfaces.Common;
using Drosy.Application.UseCases.PlanStudents.DTOs;
using Drosy.Application.UseCases.PlanStudents.Interfaces;
using Drosy.Application.UseCases.Students.Interfaces;
using Drosy.Domain.Entities;
using Drosy.Domain.Interfaces.Common.Uow;
using Drosy.Domain.Interfaces.Repository;
using Drosy.Domain.Shared.ApplicationResults;
using Drosy.Domain.Shared.ErrorComponents;
using Drosy.Domain.Shared.ErrorComponents.Common;
using Drosy.Domain.Shared.ErrorComponents.EFCore;

namespace Drosy.Application.UseCases.PlanStudents.Services
{
    public class PlanStudentsService : IPlanStudentsService
    {
        private readonly IMapper _mapper;
        private readonly IPlanStudentsRepository _planStudentRepository;
        private readonly IStudentService _studentService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<PlanStudentsService> _logger;

        public PlanStudentsService(IPlanStudentsRepository planStudentRepository, IStudentService studentService, ILogger<PlanStudentsService> logger, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _planStudentRepository = planStudentRepository;
            _studentService = studentService;
            //_planService = planService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result<PlanStudentDto>> GetById(int planId, int studentId, CancellationToken ct)
        {
            _logger.LogInformation("Starting GetById for PlanId={PlanId}, StudentId={StudentId}", planId, studentId);

            try
            {
                ct.ThrowIfCancellationRequested();

                PlanStudent? entity = await _planStudentRepository.GetById(planId,studentId, ct);

                if (entity != null)
                {
                    _logger.LogInformation("Student {StudentId} not in Plan {PlanId}", studentId, planId);
                    return Result.Failure<PlanStudentDto>(CommonErrors.NotFound);
                }

                var dto = _mapper.Map<PlanStudent,PlanStudentDto>(entity!);

                return Result.Success(dto);
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Operation canceled in GetById for PlanId={PlanId}", planId);
                return Result.Failure<PlanStudentDto>(CommonErrors.OperationCancelled);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Unexpected error in GetById for PlanId={PlanId}, StudentId={StudentId}", planId, studentId);
                return Result.Failure<PlanStudentDto>(AppError.Failure);
            }
        }

        public async Task<Result> IsStudentInPlanAsync(int planId, int studentId, CancellationToken ct)
        {
            _logger.LogInformation("Starting IsStudentInPlanAsync for PlanId={PlanId}, StudentId={StudentId}", planId, studentId);

            try
            {
                ct.ThrowIfCancellationRequested();

                var isExists = await _planStudentRepository.ExistsAsync(planId,studentId, ct);

                if (!isExists)
                {
                    _logger.LogInformation("Student {StudentId} not in Plan {PlanId}", studentId, planId);
                    return Result.Failure(CommonErrors.NotFound);
                }

                return Result.Success();
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Operation canceled in IsStudentInPlanAsync for PlanId={PlanId}", planId);
                return Result.Failure<PlanStudentDto>(CommonErrors.OperationCancelled);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Unexpected error in IsStudentInPlanAsync for PlanId={PlanId}, StudentId={StudentId}", planId, studentId);
                return Result.Failure(AppError.Failure);
            }
        }

        public async Task<Result<PlanStudentDto>> AddStudentToPlanAsync(int planId, AddStudentToPlanDto dto, CancellationToken ct)
        {
            _logger.LogInformation("Starting AddStudentToPlanAsync for PlanId={PlanId}, StudentId={StudentId}", planId, dto.StudentId);
            try
            {
                ct.ThrowIfCancellationRequested();

                #region Validations
                // 1) Plan exists
                //var plaResultn = await _planService.GetByIdAsync(planId, ct);
                //if (planResult.IsSuccess)
                //    return Result.Failure<PlanStudentDto>(Error.NotFound, new Exception("Plan not find for assining to it a student."));

                // 2) Student exists
                var studentResult = await _studentService.GetByIdAsync(dto.StudentId, ct);
                if (!studentResult.IsSuccess)
                {
                    _logger.LogWarning("Student not found: {StudentId}", dto.StudentId);
                    return Result.Failure<PlanStudentDto>(CommonErrors.NotFound, new Exception("Student not find for assining it to a plan."));
                }

                //3) No duplicate
                bool existing = await _planStudentRepository.ExistsAsync(planId, dto.StudentId, ct);
                if (existing)
                {
                    _logger.LogWarning("Student {StudentId} already assigned to Plan {PlanId}", dto.StudentId, planId);
                    return Result.Failure<PlanStudentDto>(CommonErrors.Conflict, new Exception("Student is already assigned to this plan."));
                }
                #endregion

                var planStudent = _mapper.Map<AddStudentToPlanDto, PlanStudent>(dto);

                await _planStudentRepository.AddAsync(planStudent, ct);
                bool isSaved = await _unitOfWork.SaveChangesAsync(ct);

                if (!isSaved)
                {
                    _logger.LogError("Failed to save changes when adding Student {StudentId} to Plan {PlanId}", dto.StudentId, planId);
                    return Result.Failure<PlanStudentDto>(EfCoreErrors.CanNotSaveChanges);
                }

                var planStudentDto = _mapper.Map<PlanStudent, PlanStudentDto>(planStudent);

                _logger.LogInformation("Successfully added Student {StudentId} to Plan {PlanId}", dto.StudentId, planId);
                return Result.Success(planStudentDto);

            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Operation canceled in AddStudentToPlanAsync for PlanId={PlanId}", planId);
                return Result.Failure<PlanStudentDto>(CommonErrors.OperationCancelled);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Unexpected error in AddStudentToPlanAsync for PlanId={PlanId}, StudentId={StudentId}", planId, dto.StudentId);
                return Result.Failure<PlanStudentDto>(AppError.Failure);
            }
        }

        public async Task<Result<DataResult<PlanStudentDto>>> AddRangeOfStudentToPlanAsync(int planId, IEnumerable<AddStudentToPlanDto> dtos, CancellationToken ct)
        {
            _logger.LogInformation("Starting AddRangeOfStudentToPlanAsync for PlanId={PlanId}, StudentCount={Count}", planId, dtos?.Count() ?? 0);
            try
            {
                ct.ThrowIfCancellationRequested();

                #region Validations
                // 1) Plan exists
                //var plaResultn = await _planService.GetByIdAsync(planId, ct);
                //if (planResult.IsSuccess)
                //    return Result.Failure<PlanStudentDto>(Error.NotFound, new Exception("Plan not find for assining to it students."));

                // 2) All students exist
                foreach (var dto in dtos!)
                {
                    var student = await _studentService.GetByIdAsync(dto.StudentId, ct);
                    if (student is null)
                    {
                        _logger.LogWarning("Student not found in range: {StudentId}", dto.StudentId);
                        return Result.Failure<DataResult<PlanStudentDto>>(CommonErrors.NotFound, new Exception($"Student with ID {dto.StudentId} not found."));
                    }
                }


                // 3) Check which students are already in the plan
                var studentIds = dtos.Select(dto => dto.StudentId).ToList();

                var existingStudentIds = await _planStudentRepository
                    .GetStudentIdsInPlanAsync(planId, studentIds, ct);

                // Filter out students that are already assigned
                var newDtos = dtos
                    .Where(dto => !existingStudentIds.Contains(dto.StudentId))
                    .ToList();

                if (!newDtos.Any())
                {
                    _logger.LogWarning("All students already assigned to Plan {PlanId}", planId);
                    return Result.Failure<DataResult<PlanStudentDto>>(CommonErrors.Conflict, new Exception("All students are already assigned to this plan."));
                }

                #endregion

                var planStudents = _mapper.Map<IEnumerable<AddStudentToPlanDto>, List<PlanStudent>>(newDtos);
                planStudents.ForEach(ps => ps.PlanId = planId);

                await _planStudentRepository.AddRangeAsync(planStudents, ct);
                bool isSaved = await _unitOfWork.SaveChangesAsync(ct);
                if (!isSaved)
                {
                    _logger.LogError("Failed to save batch add for PlanId={PlanId}", planId);
                    return Result.Failure<DataResult<PlanStudentDto>>(EfCoreErrors.CanNotSaveChanges);
                }

                var planStudentsDtos = _mapper.Map<List<PlanStudent>, List<PlanStudentDto>>(planStudents);
                var dataResult = new DataResult<PlanStudentDto>
                {
                    Data = planStudentsDtos ?? Enumerable.Empty<PlanStudentDto>(),
                    TotalRecordsCount = planStudentsDtos!.Count
                };

                _logger.LogInformation("Successfully added {Count} students to Plan {PlanId}", dataResult.TotalRecordsCount, planId);
                return Result.Success(dataResult);

            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Operation canceled in AddRangeOfStudentToPlanAsync for PlanId={PlanId}", planId);
                return Result.Failure<DataResult<PlanStudentDto>>(CommonErrors.OperationCancelled);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Unexpected error in AddRangeOfStudentToPlanAsync for PlanId={PlanId}", planId);
                return Result.Failure<DataResult<PlanStudentDto>>(CommonErrors.Failure);
            }
        }

    }
}