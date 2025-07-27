using System.Numerics;
using Drosy.Application.Interfaces.Common;
using Drosy.Application.UseCases.PlanStudents.DTOs;
using Drosy.Application.UseCases.Students.DTOs;
using Drosy.Application.UseCases.Students.Interfaces;
using Drosy.Domain.Entities;
using Drosy.Domain.Enums;
using Drosy.Domain.Interfaces.Repository;
using Drosy.Application.Interfaces.Common;
using Drosy.Domain.Interfaces.Common.Uow;
using Drosy.Domain.Shared.ApplicationResults;
using Drosy.Domain.Shared.ErrorComponents;
using Drosy.Domain.Shared.ErrorComponents.Common;
using Drosy.Domain.Shared.ErrorComponents.EFCore;

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
                    return Result.Failure<StudentDTO>(EfCoreErrors.CanNotSaveChanges);
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

                return isSuccess ? Result.Success() : Result.Failure(EfCoreErrors.CanNotSaveChanges);
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

        public async Task<Result<List<StudentCardInfoDTO>>> GetAllStudentsInfoCardsAsync(CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                var results = await _studentRepository.GetAllStudentsInfoCardsAsync(cancellationToken);
                var newLists = new List<StudentCardInfoDTO>();
                foreach (var s in results)
                {
                    int totalplans = s.Plans.Count(x => x.Plan.Status == PlanStatus.Active);
                    newLists.Add(new StudentCardInfoDTO
                    {
                        Address = s.Address,
                        FullName = $"{s.FirstName} {s.ThirdName} {s.LastName}",
                        Grade = s.Grade.Name,
                        PhoneNumber = s.PhoneNumber,
                        PlansCount = totalplans,
                        SessionsCount = _CalculateTotalSessionsForStudent(s)
                    });
                }


                return newLists.Count > 0 ? Result.Success(newLists) : Result.Failure<List<StudentCardInfoDTO>>(CommonErrors.NotFound);

            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Operation canceled while retrving students info");
                return Result.Failure<List<StudentCardInfoDTO>>(CommonErrors.OperationCancelled);
            }


        }

        private int _CalculateTotalSessions(Plan plan)
        {
            // 1. عدد الأيام في الأسبوع اللي فيها حصص
            int sessionsPerWeek =  0/*Enum.GetValues(typeof(DayOfWeek))
                .Cast<DayOfWeek>()
                .Count(day => plan.DaysOfWeek.HasFlag((Days)(1 << (int)day)))*/;

            // 2. عدد الأسابيع بين البداية والنهاية
            int totalDays = (plan.EndDate - plan.StartDate).Days + 1;
            double totalWeeks = totalDays / 7.0;

            // 3. الحصص الكلية = الأيام بالأسبوع × عدد الأسابيع
            int totalSessions = (int)Math.Floor(totalWeeks * sessionsPerWeek);

            return totalSessions;
        }

        private int _CalculateTotalSessionsForStudent(Student student)
        {
            int sessions = 0;
            foreach(PlanStudent planStudent in student.Plans)
            {
                sessions += _CalculateTotalSessions(planStudent.Plan);
            }
            return sessions;
        }

        public async Task<Result> ArchiveStudentAsync(int id, CancellationToken ct)
        {
            try
            {
                ct.ThrowIfCancellationRequested();
                var student = await _studentRepository.GetByIdAsync(id, ct);

                if (student is null)
                    return Result.Failure(CommonErrors.NullValue);

                await _studentRepository.SoftDeleteAsync(student, ct);
                var isSaved = await _unitOfWork.SaveChangesAsync(ct);

                return isSaved ? Result.Success() : Result.Failure(CommonErrors.Failure);
            }
            catch (OperationCanceledException)
            {
                _logger.LogError("Operation canceled while archving the student {id}", id);
                return Result.Failure(CommonErrors.OperationCancelled);
            }
        }
    }
}
