using Drosy.Application.UseCases.Attendences.DTOs;
using Drosy.Domain.Enums;
using Drosy.Domain.Shared.ApplicationResults;

namespace Drosy.Application.UseCases.Attendences.Interfaces
{
    public interface IAttendencesService
    {
        #region Read

        #endregion
        #region Write

        #endregion
        Task<Result<DataResult<AttendenceDto>>> GetAllForSessionAsync(int sessionId, CancellationToken ct);
        Task<Result<DataResult<AttendenceDto>>> GetAllForStudentAsync(int sessionId,int studentId, CancellationToken ct);
        Task<Result<DataResult<AttendenceDto>>> GetAllForStudentByStatusAsync(int sessionId,int studentId, AttendenceStatus status, CancellationToken ct);

        Task<Result<AttendenceDto>> AddAsync(int sessionId,AddAttendencenDto dto, CancellationToken ct);
        Task<Result<DataResult<AttendenceDto>>> AddRangeAsync(int sessionId,IEnumerable<AddAttendencenDto> dto, CancellationToken ct);
        Task<Result<AttendenceDto>> UpdateAsync(int sessionId, UpdateAttendencenDto dto, CancellationToken ct);
    }
}

