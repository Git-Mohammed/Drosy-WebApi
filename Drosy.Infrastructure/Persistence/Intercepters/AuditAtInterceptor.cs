using Drosy.Domain.Interfaces.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Drosy.Infrastructure.Persistence.Intercepters
{
    public class AuditAtInterceptor : SaveChangesInterceptor
    {
        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            SetAuditFields(eventData.Context);
            return base.SavingChanges(eventData, result);
        }

        public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            await Task.Run(() => SetAuditFields(eventData.Context));
            return result;
        }

        private void SetAuditFields(DbContext? context)
        {
            if (context == null) return;
            var now = DateTime.UtcNow;

            foreach (var entry in context.ChangeTracker.Entries())
            {
                if (entry.Entity is ICreateAt created && entry.State == EntityState.Added)
                    created.CreatedAt = now;

                //if (entry.Entity is IUpdateAt updated && entry.State == EntityState.Modified)
                //    updated.UpdatedAt = now;
            }
        }
    }

}
