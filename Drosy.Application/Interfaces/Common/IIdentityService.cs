using Drosy.Application.UsesCases.Users.DTOs;
using Drosy.Domain.Entities;
using Drosy.Domain.Shared.ApplicationResults;

namespace Drosy.Application.Interfaces.Common
{
    public interface IIdentityService
    {
        Task<bool> CreateUserAsync(string username, string password);
        Task<Result<AppUser>> PasswordSignInAsync(string username, string password, bool isPersistent, bool lockoutOnFailure);
        Task<Result> ChangePasswordAsync(int userId, string oldPassword, string newPassword);
        Task<Result> ForgetPasswordAsync(string email, string link, CancellationToken ct);
        Task<Result> RestPasswordAsync(RestPasswordDTO dto, CancellationToken ct);
    }
}