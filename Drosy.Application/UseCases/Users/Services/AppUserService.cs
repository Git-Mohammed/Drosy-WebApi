using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Drosy.Application.Interfaces.Common;
using Drosy.Application.UseCases.Students.DTOs;
using Drosy.Application.UseCases.Users.Interfaces;
using Drosy.Domain.Entities;
using Drosy.Domain.Interfaces.Repository;
using Drosy.Domain.Shared.ApplicationResults;
using Drosy.Domain.Shared.ErrorComponents.Common;

namespace Drosy.Application.UseCases.Users.Services
{
    public class AppUserService(IAppUserRepository appUserRepository, ILogger<AppUserService> logger) : IAppUserService
    {
        private readonly IAppUserRepository _appUserRepository = appUserRepository;
        private readonly ILogger<AppUserService> _logger = logger;
        public async Task<Result<AppUser>> FindByIdAsync(int id, CancellationToken ct)
        {
            try
            {
                ct.ThrowIfCancellationRequested();
                var user = await _appUserRepository.FindByIdAsync(id, ct);

                if (user == null) 
                    return Result.Failure<AppUser>(CommonErrors.NotFound);

                return Result.Success(user);
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Operation canceled while retrving userId: {userId} info", id);
                return Result.Failure<AppUser>(CommonErrors.OperationCancelled);
            }
        }
    }
}
