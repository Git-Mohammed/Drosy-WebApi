using Drosy.Application.UseCases.Email.DTOs;
using Drosy.Domain.Shared.ApplicationResults;

namespace Drosy.Application.UseCases.Email.Interfaces
{
    public interface IEmailService
    {
        Task<Result> SendEmailAsync(EmailMessageDTO email, CancellationToken ct);
    }
}
