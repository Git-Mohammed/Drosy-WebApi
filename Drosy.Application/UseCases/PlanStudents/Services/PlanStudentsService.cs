using Drosy.Application.Interfaces.Common;
using Drosy.Application.UseCases.PlanStudents.DTOs;
using Drosy.Application.UseCases.PlanStudents.Interfaces;
using Drosy.Application.UseCases.Students.DTOs;
using Drosy.Domain.Entities;
using Drosy.Domain.Interfaces.Repository;
using Drosy.Domain.Interfaces.Uow;
using Drosy.Domain.Shared.ResultPattern;
using Drosy.Domain.Shared.ResultPattern.ErrorComponents;

namespace Drosy.Application.UseCases.PlanStudents.Services
{
    public class PlanStudentsService : IPlanStudentsService
    {
        private readonly IMapper _mapper;
        private readonly IPlanStudentsRepository _planStudentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public PlanStudentsService(IPlanStudentsRepository planStudentRepository, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _planStudentRepository = planStudentRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> AddRangeOfStudentToPlanAsync(int planId, IEnumerable<AddStudentToPlanDto> dtos, CancellationToken ct)
        {
            try
            {
                ct.ThrowIfCancellationRequested();

                var planStudents = _mapper.Map<IEnumerable<AddStudentToPlanDto>, List<PlanStudent>>(dtos);
                planStudents.ForEach(ps => ps.PlanId = planId);

                // Update this later
                //List<PlanStudent> PlanStudents = new();
                //foreach (var dto in dtos)
                //{
                //    var planStudent = _mapper.Map<AddStudentToPlanDto, PlanStudent>(dto);
                //    planStudent.PlanId = planId;
                //    PlanStudents.Add(planStudent);
                //}


                await _planStudentRepository.AddRangeAsync(planStudents, ct);
                await _unitOfWork.SaveChangesAsync(ct);

                //List<PlanStudentDto> planStudentsDto = new();
                //foreach (var ps in planStudents)
                //    planStudentsDto.Add(_mapper.Map<PlanStudent, PlanStudentDto>(ps));

                var resultDtos = _mapper.Map<List<PlanStudent>, List<PlanStudentDto>>(planStudents);

                return Result.Success(resultDtos);
            }
            catch (OperationCanceledException)
            {
                return Result.Failure<PlanStudentDto>(Error.OperationCancelled);
            }
            catch (Exception ex)
            {
                // Log the exception
                return Result.Failure<PlanStudentDto>(Error.Failure);

            }
        }

        public async Task<Result<PlanStudentDto>> AddStudentToPlanAsync(int planId, AddStudentToPlanDto dto, CancellationToken ct) 
        {
            try
            {
                ct.ThrowIfCancellationRequested();

                var planStudent = _mapper.Map<AddStudentToPlanDto, PlanStudent>(dto);

                await _planStudentRepository.AddAsync(planStudent,ct);
                await _unitOfWork.SaveChangesAsync(ct);

                var planStudentDto = _mapper.Map<PlanStudent, PlanStudentDto>(planStudent);

                return Result.Success(planStudentDto);

            }
            catch (OperationCanceledException)
            {
                return Result.Failure<PlanStudentDto>(Error.OperationCancelled);
            }
            catch (Exception ex)
            {
                // Log
                return Result.Failure<PlanStudentDto>(Error.Failure);
            }
        }


       

    }
}
