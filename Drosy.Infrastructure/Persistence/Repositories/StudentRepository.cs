using Drosy.Domain.Entities;
using Drosy.Domain.Interfaces.Repository;
using Drosy.Infrastructure.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Drosy.Infrastructure.Persistence.Repositories
{
    public class StudentRepository : BaseRepository<Student>, IStudentRepository
    {
        public StudentRepository(ApplicationDbContext dbContext) : base(dbContext) { }
        public Student? GetById(int id)
        {
           return DbSet.FirstOrDefault(x => x.Id ==  id);
        }

        public async Task<Student?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await DbSet.Include(x => x.City).Include(x => x.Grade).FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken)
        {
            return await DbSet.AnyAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<List<Student>> GetAllStudentsInfoCardsAsync(int page, int size, CancellationToken cancellationToken)
        {
            return await DbSet.Include(x => x.Grade).Include(x => x.Plans).ToListAsync(cancellationToken);
        }
    }
}