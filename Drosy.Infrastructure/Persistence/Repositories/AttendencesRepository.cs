using Drosy.Domain.Entities;
using Drosy.Domain.Interfaces.Repository;
using Drosy.Infrastructure.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;
using System.Threading;

namespace Drosy.Infrastructure.Persistence.Repositories
{
    public class AttendencesRepository(ApplicationDbContext dbContext) : BaseRepository<Attendence>(dbContext), IAttendencesRepository
    {
        public async Task<bool> ExistsAsync(int sessionId, int studentId, CancellationToken ct)
        {
            return await DbSet.AnyAsync(a => a.SessionId == sessionId & a.StudentId == studentId, ct);
        }

        public async Task<Attendence?> GetByIdAsync(int sessionId, int studentId, CancellationToken ct)
        {
            return await DbSet.Include(x => x.Student).Include(x => x.Session).FirstOrDefaultAsync(x => x.SessionId == sessionId & x.StudentId == studentId, ct);
        }

        public async Task<IEnumerable<Attendence>> GetAllForStudentBySessionAsync(int sessionId, IEnumerable<int> studentIds, CancellationToken ct)
        {
            return await DbSet
            .Where(a => a.SessionId == sessionId && studentIds.Contains(a.StudentId))
            .ToListAsync(ct);
        }
    }
}
