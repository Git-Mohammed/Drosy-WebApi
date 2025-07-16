using Drosy.Application.Interfaces.Common;
using Drosy.Domain.Entities;
using Drosy.Domain.Shared.ApplicationResults;
using Drosy.Domain.Shared.ErrorComponents.User;
using Drosy.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Identity;

namespace Drosy.Infrastructure.Identity
{
    /*
     TODO:
        - Check Password and Then Check LouckOut
     */
    public class IdentityService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        RoleManager<ApplicationRole> roleManager, IMapper mapper)
        : IIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager = roleManager;
        private readonly IMapper _mapper = mapper;

        public Task<bool> CreateUserAsync(string username, string password)
        {
            _userManager.CreateAsync(new ApplicationUser { UserName = username, Email = username }, password);
            return Task.FromResult(true);
        }

        public async Task<Result<AppUser>> PasswordSignInAsync(string username, string password, bool isPersistent, bool lockoutOnFailure)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user is null)
                return Result.Failure<AppUser>(UserErrors.InvalidCredentials);

            var result = await _signInManager.PasswordSignInAsync(user, password, isPersistent, lockoutOnFailure);
            if (result.IsLockedOut)
                return Result.Failure<AppUser>(UserErrors.AttemptExceeded);
                    
            if (!result.Succeeded)
                return Result.Failure<AppUser>(UserErrors.InvalidCredentials);
            
            return Result.Success(_mapper.Map<ApplicationUser, AppUser>(user));
        }
    }
}