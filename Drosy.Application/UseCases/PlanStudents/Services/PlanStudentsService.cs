using Drosy.Application.Interfaces.Common;
using Drosy.Application.UseCases.PlanStudents.DTOs;
using Drosy.Application.UseCases.PlanStudents.Interfaces;
using Drosy.Application.UseCases.Students.DTOs;
using Drosy.Application.UseCases.Students.Interfaces;
using Drosy.Domain.Entities;
using Drosy.Domain.Interfaces.Repository;
using Drosy.Domain.Interfaces.Uow;
using Drosy.Domain.Shared.DataDTOs;
using Drosy.Domain.Shared.ResultPattern;
using Drosy.Domain.Shared.ResultPattern.ErrorComponents;

namespace Drosy.Application.UseCases.PlanStudents.Services
{
    public class PlanStudentsService : IPlanStudentsService
    {
        private readonly IMapper _mapper;
        private readonly IPlanStudentsRepository _planStudentRepository;
        private readonly IStudentService _studentService;
        private readonly IUnitOfWork _unitOfWork;

        public PlanStudentsService(IPlanStudentsRepository planStudentRepository,IStudentService studentService, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _planStudentRepository = planStudentRepository;
            _studentService = studentService;
            //_planService = planService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<PlanStudentDto>> AddStudentToPlanAsync(int planId, AddStudentToPlanDto dto, CancellationToken ct)
        {
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
                    return Result.Failure<PlanStudentDto>(Error.NotFound, new Exception("Student not find for assining it to a plan."));

                //3) No duplicate
                bool alreadyInPlan = await _planStudentRepository.ExistsAsync(ps => ps.PlanId == planId && ps.StudentId == dto.StudentId, ct);
                if (alreadyInPlan)
                    return Result.Failure<PlanStudentDto>(Error.Conflict, new Exception("Student is already assigned to this plan."));
                #endregion

                var planStudent = _mapper.Map<AddStudentToPlanDto, PlanStudent>(dto);

                await _planStudentRepository.AddAsync(planStudent, ct);
                bool isSaved= await _unitOfWork.SaveChangesAsync(ct);

                var planStudentDto = _mapper.Map<PlanStudent, PlanStudentDto>(planStudent);

                return isSaved ? Result.Success(planStudentDto) :  Result.Failure<PlanStudentDto>(Error.EFCore.CanNotSaveChanges);

            }
            catch (OperationCanceledException)
            {
                return Result.Failure<PlanStudentDto>(Error.OperationCancelled);
            }
            catch (Exception)
            {
                // Log
                return Result.Failure<PlanStudentDto>(Error.Failure);
            }
        }

        public async Task<Result<DataResult<PlanStudentDto>>> AddRangeOfStudentToPlanAsync(int planId, IEnumerable<AddStudentToPlanDto> dtos, CancellationToken ct)
        {
            try
            {
                ct.ThrowIfCancellationRequested();

                #region Validations
                // 1) Plan exists
                //var plaResultn = await _planService.GetByIdAsync(planId, ct);
                //if (planResult.IsSuccess)
                //    return Result.Failure<PlanStudentDto>(Error.NotFound, new Exception("Plan not find for assining to it students."));

                // 2) All students exist
                foreach (var dto in dtos)
                {
                    var student = await _studentService.GetByIdAsync(dto.StudentId, ct);
                    if (student is null)
                        return Result.Failure<DataResult<PlanStudentDto>>(Error.NotFound, new Exception($"Student with ID {dto.StudentId} not found."));
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
                    return Result.Failure<DataResult<PlanStudentDto>>(Error.Conflict, new Exception("All students are already assigned to this plan."));

                #endregion

                var planStudents = _mapper.Map<IEnumerable<AddStudentToPlanDto>, List<PlanStudent>>(newDtos);
                planStudents.ForEach(ps => ps.PlanId = planId);

                await _planStudentRepository.AddRangeAsync(planStudents, ct);
                bool isSaved =  await _unitOfWork.SaveChangesAsync(ct);

                var resultDtos = _mapper.Map<List<PlanStudent>, List<PlanStudentDto>>(planStudents);

                return isSaved 
                    ? Result<DataResult<PlanStudentDto>>.Success(new DataResult<PlanStudentDto>
                    {
                        Data = resultDtos ?? Enumerable.Empty<PlanStudentDto>(),
                        TotalRecordsCount = resultDtos!.Count
                    })
                    : Result.Failure<DataResult<PlanStudentDto>>(Error.EFCore.CanNotSaveChanges);

            }
            catch (OperationCanceledException)
            {
                return Result.Failure<DataResult<PlanStudentDto>>(Error.OperationCancelled);
            }
            catch (Exception)
            {
                // Log the exception
                return Result.Failure<DataResult<PlanStudentDto>>(Error.Failure);
            }
        }

    }
}