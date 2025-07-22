using Drosy.Domain.Entities;
using Drosy.Domain.Enums;
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
             => await DbSet
                  .Include(a => a.Student)
                  .Include(a => a.Session)
                  .FirstOrDefaultAsync(a => a.SessionId == sessionId && a.StudentId == studentId, ct);

        public async Task<IEnumerable<Attendence>> GetAllForStudentBySessionAsync(int sessionId, IEnumerable<int> studentIds, CancellationToken ct)
            => await DbSet
                 .Include(a => a.Student)
                 .Include(a => a.Session)
                 .Where(a => a.SessionId == sessionId && studentIds.Contains(a.StudentId))
                 .ToListAsync(ct);

      
        public async Task<IEnumerable<Attendence>> GetAllForSessionAsync(int sessionId, CancellationToken ct)
            => await DbSet
                 .Include(a => a.Student)
                 .Include(a => a.Session)
                 .Where(a => a.SessionId == sessionId)
                 .ToListAsync(ct);

        public async Task<IEnumerable<Attendence>> GetAllForStudentAsync(int sessionId, int studentId, CancellationToken ct)
            => await DbSet
                 .Include(a => a.Student)
                 .Include(a => a.Session)
                 .Where(a => a.SessionId == sessionId && a.StudentId == studentId)
                 .ToListAsync(ct);

        public async Task<IEnumerable<Attendence>> GetAllForSessionByStatusAsync(int sessionId, AttendenceStatus status, CancellationToken ct)
            => await DbSet
                 .Include(a => a.Student)
                 .Include(a => a.Session)
                 .Where(a
                     => a.SessionId == sessionId
                     && a.Status == status)
                 .ToListAsync(ct);
    }
}
