using Drosy.Application.Interfaces;
using Drosy.Application.Interfaces.Common;
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
    }
}