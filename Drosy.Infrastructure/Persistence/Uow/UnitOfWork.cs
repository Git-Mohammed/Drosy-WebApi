using Drosy.Domain.Interfaces.Uow;
using Drosy.Infrastructure.Persistence.DbContexts;

namespace Drosy.Infrastructure.Persistence.Uow
{
    public class UnitOfWork(ApplicationDbContext context) : IUnitOfWork
    {
        private readonly ApplicationDbContext _context = context;
        public async Task<bool> SaveChangesAsync(CancellationToken cancellationToken) =>
            await _context.SaveChangesAsync(cancellationToken) > 0;
    }
}
