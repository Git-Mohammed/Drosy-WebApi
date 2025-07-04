namespace Drosy.Application.Interfaces.Common
{
    public interface IIdentityService
    {
        Task<bool> CreateUserAsync(string username, string password);
    }
}