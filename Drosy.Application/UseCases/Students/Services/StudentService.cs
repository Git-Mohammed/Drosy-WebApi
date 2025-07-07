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
    public class StudentService : IStudentService
    {
        private readonly IMapper _mapper;
        private readonly IStudentRepository _studentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public StudentService(IStudentRepository studentRepository, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _studentRepository = studentRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<StudentDTO>> AddAsync(AddStudentDTO dto)
        {
            try
            {
                var student = _mapper.Map<AddStudentDTO, Student>(dto);

                await _studentRepository.AddAsync(student);
               await _unitOfWork.SaveChangesAsync(CancellationToken.None);

                var studentDto = _mapper.Map<Student, StudentDTO>(student);

                return Result.Success(studentDto);

            }
            catch (Exception ex)
            {
                // Log the exception
                return Result.Failure<StudentDTO>(Error.Failure);

            }
        }

        public async Task<Result<StudentDTO>> GetByIdAsync(int id)
        {
            try
            {

                var student = await _studentRepository.GetByIdAsync(id);

                if (student == null)
                {
                    return Result.Failure<StudentDTO>(Error.NotFound);
                }

                var studentDto = _mapper.Map<Student, StudentDTO>(student);

                return Result.Success(studentDto);

            }
            catch (Exception ex)
            {
                // Log the exception
                return Result.Failure<StudentDTO>(Error.Failure);

            }
            
        }
    }
}
