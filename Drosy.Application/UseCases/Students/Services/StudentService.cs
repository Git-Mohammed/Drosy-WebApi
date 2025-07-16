using Drosy.Application.Interfaces.Common;
using Drosy.Application.UseCases.Students.DTOs;
using Drosy.Application.UseCases.Students.Interfaces;
using Drosy.Domain.Entities;
using Drosy.Domain.Interfaces.Common.Uow;
using Drosy.Domain.Interfaces.Repository;
using Drosy.Domain.Shared.ApplicationResults;
using Drosy.Domain.Shared.ErrorComponents;
using Drosy.Domain.Shared.ErrorComponents.EFCoreErrors;
using Drosy.Domain.Shared.ResultPattern.ErrorComponents.Common;

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
                    return Result.Failure<StudentDTO>(CommonErrors.NotFound);
                }

                var studentDto = _mapper.Map<Student, StudentDTO>(student);

                return Result.Success(studentDto);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error adding student: {Message}", ex.Message);
                return Result.Failure<StudentDTO>(CommonErrors.Failure);

            }
        }

        public async Task<Result<StudentDTO>> AddAsync(AddStudentDTO dto, CancellationToken ct)
        {
            try
            {
                var student = _mapper.Map<AddStudentDTO, Student>(dto);

                await _studentRepository.AddAsync(student, ct);

                bool isSuccess = await _unitOfWork.SaveChangesAsync(ct);

                if (!isSuccess)
                {
                    return Result.Failure<StudentDTO>(EFCoreErrors.CanNotSaveChanges);
                }

                var studentDto = _mapper.Map<Student, StudentDTO>(student);

                return Result.Success(studentDto);
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError("Error adding student: {Message}", ex.Message);
                return Result.Failure<StudentDTO>(AppError.Failure);

            }

        }

        public async Task<Result> UpdateAsync(UpdateStudentDTO dto, int id, CancellationToken ct)
        {
            try
            {
                var student = await _studentRepository.GetByIdAsync(id, ct);
                if (student == null)
                {
                    return Result.Failure(CommonErrors.NotFound);
                }

                // Map the DTO to the entity
                _mapper.Map(dto, student);

                await _studentRepository.UpdateAsync(student, ct);

                bool isSuccess = await _unitOfWork.SaveChangesAsync(ct);

                return isSuccess ? Result.Success() : Result.Failure(EFCoreErrors.CanNotSaveChanges);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error updating student: {Message}", ex.Message);
                return Result.Failure(AppError.Failure);
            }
        }

        public async Task<Result> ExistsAsync(int id, CancellationToken cancellationToken)
        {
            try
            {
                var isExists = await _studentRepository.ExistsAsync(id, cancellationToken);

                return isExists ? Result.Success() : Result.Failure(CommonErrors.NotFound);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return Result.Failure(AppError.Failure);
            }
        }

        public async Task<Result<List<StudentCardInfoDTO>>> GetAllStudentsInfoCardsAsync(int page, int size, CancellationToken cancellationToken)
        {
            //try
            //{
            //    cancellationToken.ThrowIfCancellationRequested();


            //}
            //catch() { }

            var results = await _studentRepository.GetAllStudentsInfoCardsAsync(1, 1, cancellationToken);

            return Result.Success(results.Select(x => _mapper.Map<Student, StudentCardInfoDTO>(x)).ToList());
        }
    }
}
