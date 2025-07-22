using Drosy.Domain.Entities;
using Drosy.Domain.Interfaces.Common.Repository;
using Drosy.Domain.Shared.ApplicationResults;

namespace Drosy.Domain.Interfaces.Repository
{
    public interface IAttendencesRepository : IRepository<Attendence>
    {
        Task<bool> ExistsAsync(int sessionId, int studentId, CancellationToken ct);
        Task<IEnumerable<Attendence>> GetAllForStudentBySessionAsync(int sessionId, IEnumerable<int> studentIds, CancellationToken ct);
        Task<Attendence?> GetByIdAsync(int sessionId, int studentId, CancellationToken ct);

    }
}
