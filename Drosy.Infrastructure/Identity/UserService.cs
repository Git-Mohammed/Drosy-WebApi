using Drosy.Application.Features.Users.Interfaces;
using Drosy.Application.Interfaces;
using Drosy.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Identity;

namespace Drosy.Infrastructure.Identity
{
    public class UserService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        RoleManager<IdentityRole> roleManager)
        : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
        private readonly RoleManager<IdentityRole> _roleManager = roleManager;



    }
}