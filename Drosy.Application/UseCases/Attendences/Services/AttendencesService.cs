
using Drosy.Application.UseCases.PlanStudents.DTOs;
using Drosy.Domain.Enums;
using Drosy.Domain.Shared.ApplicationResults;

namespace Drosy.Application.UseCases.Attendences.Services
{
    public interface IAttendencesService
    {
        Task<Result<DataResult<AttendenceDto>>> GetAllForSessionAsync(int sessionId, CancellationToken ct);
        Task<Result<DataResult<AttendenceDto>>> GetAllForStudentAsync(int studentId, CancellationToken ct);
        Task<Result<DataResult<AttendenceDto>>> GetAllForStudentByStatusAsync(int studentId, AttendenceStatus status, CancellationToken ct);

        Task<Result<AttendenceDto>> AddAsync(AddAttendencenDto dto, CancellationToken ct);
        Task<Result<DataResult<AttendenceDto>>> AddRangeAsync(IEnumerable<AddAttendencenDto> dto, CancellationToken ct);
        Task<Result<AttendenceDto>> UpdateAsync(AddAttendencenDto dto, CancellationToken ct);
    }

    public class AttendencesService : IAttendencesService
    {
    }
    public class AttendenceDto()
    {

    }
    public class AddAttendencenDto()
    {

    }
}

