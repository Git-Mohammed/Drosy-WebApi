using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Drosy.Domain.Entities;
using Drosy.Domain.Shared.ApplicationResults;

namespace Drosy.Application.UseCases.Users.Interfaces
{
    public interface IAppUserService
    {
        Task<Result<AppUser>> FindByIdAsync(int id, CancellationToken ct);
    }
}
