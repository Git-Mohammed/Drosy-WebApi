using Drosy.Domain.Entities;
using Drosy.Domain.Interfaces.Repository;
using Drosy.Infrastructure.Identity.Entities;
using Drosy.Infrastructure.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Drosy.Infrastructure.Persistence.Repositories
{
    public class StudentRepository : BaseRepository<Student>, IStudentRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly DbSet<Student> _dbSet;
        public StudentRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Students;
        }

        public Student? GetById(int id)
        {
           return _dbSet.FirstOrDefault(x => x.Id ==  id);
        }

        public async Task<Student?> GetByIdAsync(int id)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}