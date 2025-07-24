using Drosy.Domain.Entities;
using Drosy.Domain.Interfaces.Common.Repository;

namespace Drosy.Domain.Interfaces.Repository;

public interface IPaymentRepository : IRepository<Payment>
{
    Task<Payment> GetByIdAsync(int id, CancellationToken cancellationToken);
}