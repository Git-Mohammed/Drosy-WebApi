using Drosy.Application.Interfaces.Common;
using Drosy.Domain.Shared.ResultPattern;
using Drosy.Domain.Shared.ResultPattern.ErrorComponents;
using Drosy.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Identity;

namespace Drosy.Infrastructure.Identity
{
    public class IdentityService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        RoleManager<ApplicationRole> roleManager)
        : IIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager = roleManager;


        public Task<bool> CreateUserAsync(string username, string password)
        {
            _userManager.CreateAsync(new ApplicationUser { UserName = username, Email = username }, password);
            return Task.FromResult(true);
        }

        public async Task<Result> PasswordSignInAsync(string username, string password, bool isPersistent, bool lockoutOnFailure)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user is null)
                return Result.Failure(Error.User.InvalidCredentials);

            var result = await _signInManager.PasswordSignInAsync(user, password, isPersistent, lockoutOnFailure);
            if (result.IsLockedOut)
                return Result.Failure(Error.User.AttempExceeded);
                    
            if (!result.Succeeded)
                return Result.Failure(Error.User.InvalidCredentials);

            return Result.Success();
        }
    }
}