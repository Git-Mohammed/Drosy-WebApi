using Drosy.Application.UseCases.Sessions.DTOs;
using Drosy.Domain.Shared.ApplicationResults;

namespace Drosy.Application.UseCases.Sessions.Interfaces;

public interface ISessionService
{
    #region Read
    public Task<Result<SessionDTO>> GetByIdAsync(int id, CancellationToken cancellationToken);
    #endregion

    #region Write 
    public Task<Result<SessionDTO>> AddAsync(AddSessionDTO sessionDTO, CancellationToken cancellationToken);
    #endregion
}
