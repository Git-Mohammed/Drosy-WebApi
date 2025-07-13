using Drosy.Domain.Interfaces.Repository;
using Drosy.Application.UseCases.Students.Interfaces;
using Drosy.Domain.Entities;
using Drosy.Application.UseCases.Students.DTOs;
using Drosy.Domain.Shared.ResultPattern;
using Drosy.Application.Interfaces.Common;
using Drosy.Domain.Shared.ResultPattern.ErrorComponents;
using Drosy.Domain.Interfaces.Uow;

namespace Drosy.Application.UseCases.Students.Services
{
    // TODO:
    // 1- Add logging  
    // 2- See AddStudentValidator for validation logic
    public class StudentService : IStudentService
    {
        private readonly IMapper _mapper;
        private readonly IStudentRepository _studentRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<StudentService> _logger;

        public StudentService(IStudentRepository studentRepository, IMapper mapper, IUnitOfWork unitOfWork, ILogger<StudentService> logger)
        {
            _studentRepository = studentRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result<StudentDTO>> GetByIdAsync(int id, CancellationToken ct)
        {
            try
            {
                var student = await _studentRepository.GetByIdAsync(id, ct);

                if (student == null)
                {
                    return Result.Failure<StudentDTO>(Error.NotFound);
                }

                var studentDto = _mapper.Map<Student, StudentDTO>(student);

                return Result.Success(studentDto);

            }
            catch (Exception ex)
            {
                _logger.LogError("Error adding student: {Message}", ex.Message);
                return Result.Failure<StudentDTO>(Error.Failure);

            }
        }

        public async Task<Result<StudentDTO>> AddAsync(AddStudentDTO dto, CancellationToken ct)
        {
            try
            { 
                var student = _mapper.Map<AddStudentDTO, Student>(dto);
                
                await _studentRepository.AddAsync(student, ct);

                bool isSuccess =  await _unitOfWork.SaveChangesAsync(ct);

                if(!isSuccess)
                {
                    return Result.Failure<StudentDTO>(Error.EFCore.CanNotSaveChanges);
                }

                var studentDto = _mapper.Map<Student, StudentDTO>(student);

                return Result.Success(studentDto);
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError("Error adding student: {Message}", ex.Message);
                return Result.Failure<StudentDTO>(Error.Failure);

            }
            
        }
  
        public async Task<Result> UpdateAsync(UpdateStudentDTO dto, int id, CancellationToken ct)
        {
            try
            {
                var student = await _studentRepository.GetByIdAsync(id, ct);
                if (student == null)
                {
                    return Result.Failure(Error.NotFound);
                }

                // Map the DTO to the entity
                _mapper.Map(dto, student);

                await _studentRepository.UpdateAsync(student, ct);

                bool isSuccess = await _unitOfWork.SaveChangesAsync(ct);

                return isSuccess ? Result.Success() : Result.Failure(Error.EFCore.CanNotSaveChanges);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error updating student: {Message}", ex.Message);
                return Result.Failure(Error.Failure);
            }
        }
    }
}
