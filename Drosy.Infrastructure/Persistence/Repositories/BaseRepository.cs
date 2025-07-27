using Drosy.Domain.Interfaces.Common;
using Drosy.Domain.Interfaces.Common.Repository;
using Drosy.Domain.Interfaces.Common.Uow;
using Drosy.Infrastructure.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Drosy.Infrastructure.Persistence.Repositories
{
    public abstract class BaseRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly DbSet<TEntity> DbSet;
        protected readonly ApplicationDbContext DbContext;

        protected BaseRepository(ApplicationDbContext dbContext)
        {
            DbSet = dbContext.Set<TEntity>();
            DbContext = dbContext;
        }
       
        public virtual async Task AddAsync(TEntity entity,CancellationToken cancellationToken)
        {
            await DbSet.AddAsync(entity, cancellationToken);
        }

        public virtual async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken)
        {
            await DbSet.AddRangeAsync(entities, cancellationToken);
        }

        public virtual async Task UpdateAsync(TEntity entity,  CancellationToken cancellationToken)
        {
            await Task.Run(() => DbSet.Update(entity), cancellationToken);
        }

        public virtual async Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken)
        {
            await Task.Run(() => DbSet.UpdateRange(entities), cancellationToken);
        }

        public virtual async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken)
        {

            await Task.Run(() => DbSet.Remove(entity), cancellationToken);
        }

        public virtual async Task DeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken)
        {
            await Task.Run(() => DbSet.RemoveRange(entities), cancellationToken);
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await DbSet.ToListAsync(cancellationToken);
        }

        public async Task SoftDeleteAsync(TEntity entity, CancellationToken ct)
        {
            if (entity is ISoftDeleteble softDeletableEntity)
            {
                softDeletableEntity.IsDeleted = true;
                await UpdateAsync(entity, ct);
            }
            else
            {
                throw new InvalidOperationException("Entity does not support soft delete.");
            }
        }
    }
}