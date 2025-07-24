using Drosy.Domain.Entities;
using Drosy.Domain.Interfaces.Repository;
using Drosy.Infrastructure.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Drosy.Infrastructure.Persistence.Repositories;

public class PaymentRepository(ApplicationDbContext dbContext) : BaseRepository<Payment>(dbContext) ,IPaymentRepository
{
    public async Task<Payment> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await DbSet.FirstOrDefaultAsync(x=>x.Id == id, cancellationToken);
    }
}