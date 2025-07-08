using Drosy.Domain.Interfaces.Repository;
using Drosy.Infrastructure.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Drosy.Infrastructure.Persistence.Repositories
{
    public abstract class BaseRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly DbSet<TEntity> DbSet;

        protected BaseRepository(ApplicationDbContext dbContext)
        {
            DbSet = dbContext.Set<TEntity>();
        }

        public virtual async Task AddAsync(TEntity entity)
        {
            await DbSet.AddAsync(entity);
        }

        public virtual async Task AddRangeAsync(IEnumerable<TEntity> entities)
        {
            await DbSet.AddRangeAsync(entities);
        }

        public virtual async Task UpdateAsync(TEntity entity)
        {
            DbSet.Update(entity);
            await Task.CompletedTask;
        }

        public virtual async Task UpdateRangeAsync(IEnumerable<TEntity> entities)
        {
            DbSet.UpdateRange(entities);
            await Task.CompletedTask;
        }

        public virtual async Task DeleteAsync(TEntity entity)
        {
            DbSet.Remove(entity);
            await Task.CompletedTask;
        }

        public virtual async Task DeleteRangeAsync(IEnumerable<TEntity> entities)
        {
            DbSet.RemoveRange(entities);
            await Task.CompletedTask;
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await DbSet.ToListAsync();
        }
    }
}