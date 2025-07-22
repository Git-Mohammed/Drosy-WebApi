using Drosy.Domain.Entities;
using Drosy.Domain.Enums;
using Drosy.Domain.Interfaces.Common.Repository;

namespace Drosy.Domain.Interfaces.Repository
{
    public interface IAttendencesRepository : IRepository<Attendence>
    {
        Task<bool> ExistsAsync(int sessionId, int studentId, CancellationToken ct);
        Task<IEnumerable<Attendence>> GetAllForStudentBySessionAsync(int sessionId, IEnumerable<int> studentIds, CancellationToken ct);
        Task<Attendence?> GetByIdAsync(int sessionId, int studentId, CancellationToken ct);
        /// <summary>All attendences for a given session.</summary>
        Task<IEnumerable<Attendence>> GetAllForSessionAsync(int sessionId, CancellationToken ct);

        /// <summary>All attendences for a given student in a given session.</summary>
        Task<IEnumerable<Attendence>> GetAllForStudentAsync(int sessionId, int studentId, CancellationToken ct);

        /// <summary>All attendences for a given student + status in a given session.</summary>
        Task<IEnumerable<Attendence>> GetAllForSessionByStatusAsync(int sessionId,  AttendenceStatus status, CancellationToken ct);
    }
}
