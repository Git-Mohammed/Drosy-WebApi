using Drosy.Domain.Entities;

namespace Drosy.Domain.Interfaces.Repository
{
    public interface IPasswordResetTokenRepository
    {
        Task AddAsync(PasswordResetToken entity, CancellationToken cancellationToken);
        Task UpdateAsync(PasswordResetToken entity, CancellationToken cancellationToken);
    }
}
