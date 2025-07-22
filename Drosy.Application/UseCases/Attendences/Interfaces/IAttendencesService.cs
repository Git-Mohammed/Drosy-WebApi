using Drosy.Application.UseCases.Attendences.DTOs;
using Drosy.Domain.Enums;
using Drosy.Domain.Shared.ApplicationResults;

namespace Drosy.Application.UseCases.Attendences.Interfaces
{
    public interface IAttendencesService
    {
        #region Read
        Task<Result<DataResult<AttendenceDto>>> GetAllForSessionAsync(int sessionId, CancellationToken ct);
        Task<Result<DataResult<AttendenceDto>>> GetAllForStudentAsync(int sessionId, int studentId, CancellationToken ct);
        Task<Result<DataResult<AttendenceDto>>> GetAllForStudentByStatusAsync(int sessionId, int studentId, AttendenceStatus status, CancellationToken ct);
        #endregion

        #region Write
        Task<Result<AttendenceDto>> AddAsync(int sessionId, AddAttendencenDto dto, CancellationToken ct);
        Task<Result<DataResult<AttendenceDto>>> AddRangeAsync(int sessionId, IEnumerable<AddAttendencenDto> dto, CancellationToken ct);
        Task<Result<AttendenceDto>> UpdateAsync(int sessionId, int studentId, UpdateAttendencenDto dto, CancellationToken ct);
        #endregion
    }
}

