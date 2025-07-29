using Drosy.Domain.Entities;
using Drosy.Domain.Interfaces.Repository;
using Drosy.Infrastructure.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Drosy.Infrastructure.Persistence.Repositories;

public class PaymentRepository(ApplicationDbContext dbContext) : BaseRepository<Payment>(dbContext) ,IPaymentRepository
{
    public async Task<Payment?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await DbSet.FirstOrDefaultAsync(x=>x.Id == id, cancellationToken);
    }

    public async Task<List<Payment>> GetStudentPaymentsAsync(int studentId, DateTime? fromDate, DateTime? toDate, CancellationToken cancellationToken)
    {
        var query = DbSet.Where(p => p.StudentId == studentId);

        if (fromDate.HasValue)
            query = query.Where(p => p.CreatedAt >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(p => p.CreatedAt <= toDate.Value);

        return await query.AsNoTracking()
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<decimal> GetStudentTotalFeeAsync(int studentId, CancellationToken cancellationToken)
    {
        return await DBContext.PlanStudents
            .Where(ps => ps.StudentId == studentId)
            .SumAsync(ps => ps.Fee, cancellationToken);
    }

}