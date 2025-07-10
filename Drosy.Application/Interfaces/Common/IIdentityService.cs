using Drosy.Domain.Entities;
using Drosy.Domain.Shared.ResultPattern;

namespace Drosy.Application.Interfaces.Common
{
    public interface IIdentityService
    {
    
        Task<bool> CreateUserAsync(string username, string password);
        Task<Result<AppUser>> PasswordSignInAsync(string username, string password, bool isPersistent, bool lockoutOnFailure);   
    }
}