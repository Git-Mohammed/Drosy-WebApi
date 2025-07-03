using Drosy.Application.Interfaces.Common;
using Drosy.Domain.Shared.System;
using Drosy.Infrastructure.Identity.Entities;
using Drosy.Infrastructure.Persistence.DbContexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Drosy.Infrastructure.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public DbInitializer(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public void Initialize()
        {
            try
            {
                if (_context.Database.GetPendingMigrations().Any())
                {
                    _context.Database.Migrate();
                }

                if (!_roleManager.RoleExistsAsync(AppUserRoles.Admin).GetAwaiter().GetResult())
                {
                    _roleManager.CreateAsync(new ApplicationRole() { Name = AppUserRoles.Admin }).GetAwaiter().GetResult();
                    _roleManager.CreateAsync(new ApplicationRole() { Name = AppUserRoles.Receptionist }).GetAwaiter().GetResult();
                    _roleManager.CreateAsync(new ApplicationRole() { Name = AppUserRoles.Student }).GetAwaiter().GetResult();

                }

                if (!_context.AppUsers.Any())
                {
                    ApplicationUser user = new();
                    user.Email = "admin@drosy.com";
                    user.UserName = "admin@drosy.com";
                    user.LockoutEnabled = false;

                    var result = _userManager.CreateAsync(user, "Admin123@").GetAwaiter().GetResult();

                    if (result.Succeeded)
                    {
                        _userManager.AddToRoleAsync(user, AppUserRoles.Admin).GetAwaiter().GetResult();

                        // Email Confirmed
                        var codeToConfirm = _userManager.GenerateEmailConfirmationTokenAsync(user).GetAwaiter().GetResult();
                        codeToConfirm = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(codeToConfirm));

                        codeToConfirm = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(codeToConfirm));
                        _userManager.ConfirmEmailAsync(user, codeToConfirm).GetAwaiter().GetResult();

                        _userManager.ConfirmEmailAsync(user, codeToConfirm).GetAwaiter().GetResult();

                        // Set Lockout Enabled to false
                        _userManager.SetLockoutEnabledAsync(user, false);
                    }
                }
            }
            catch (Exception ex)
            {
                //Log.Error(ex.Message);
                throw new Exception($"Something got wrong while initializing the database: {ex.Message}");
            }
        }
    }
}
