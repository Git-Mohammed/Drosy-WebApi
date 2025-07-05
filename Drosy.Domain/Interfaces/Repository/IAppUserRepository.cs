using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Drosy.Domain.Entities;

namespace Drosy.Domain.Interfaces.Repository
{
    public interface IAppUserRepository : IRepository<AppUser>
    {
        Task<AppUser> FindByIdAsync(int id);
        Task<AppUser?> FindByUsernameAsync(string userName);
    }
}
