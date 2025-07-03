using Drosy.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Drosy.Infrastructure.Persistence.DbContext
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : IdentityDbContext<ApplicationUser,ApplicationRole,int>(options)
    {
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

            #region Rename Identity Tables Names
            builder.Entity<ApplicationUser>().ToTable("Users", "Identity");
            builder.Entity<ApplicationRole>().ToTable("Roles", "Identity");
            builder.Entity<IdentityUserRole<int>>().ToTable("UserRoles", "Identity");
            builder.Entity<IdentityUserClaim<int>>().ToTable("UserClaims", "Identity");
            builder.Entity<IdentityUserLogin<int>>().ToTable("UserLogins", "Identity");
            builder.Entity<IdentityRoleClaim<int>>().ToTable("RoleClaims", "Identity");
            builder.Entity<IdentityUserToken<int>>().ToTable("UserTokens", "Identity");
            #endregion
        }
    }
}