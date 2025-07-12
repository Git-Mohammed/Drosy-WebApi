using Drosy.Domain.Entities;
using Drosy.Domain.Interfaces.Repository;
using Drosy.Infrastructure.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Drosy.Infrastructure.Persistence.Repositories
{
    public class PlanStudentsRepository : BaseRepository<PlanStudent>, IPlanStudentsRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly DbSet<PlanStudent> _dbSet;

        public PlanStudentsRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.PlanStudents;
        }


    }
}
