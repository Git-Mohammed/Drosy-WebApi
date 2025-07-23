using Drosy.Domain.Entities;
using Drosy.Domain.Interfaces.Repository;
using Drosy.Infrastructure.Persistence.DbContexts;

namespace Drosy.Infrastructure.Persistence.Repositories;

public class PaymentRepository(ApplicationDbContext dbContext) : BaseRepository<Payment>(dbContext) ,IPaymentRepository
{
    
}