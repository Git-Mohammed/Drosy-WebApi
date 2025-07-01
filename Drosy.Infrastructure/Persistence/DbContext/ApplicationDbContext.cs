using Drosy.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Drosy.Infrastructure.Persistence.DbContext
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : IdentityDbContext<ApplicationUser>(options)
    {
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

            #region Rename Identity Tables Names
            builder.Entity<ApplicationUser>().ToTable("Users", "Identity");
            builder.Entity<IdentityRole>().ToTable("Roles", "Identity");
            builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles", "Identity");
            builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims", "Identity");
            builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins", "Identity");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims", "Identity");
            builder.Entity<IdentityUserToken<string>>().ToTable("UserTokens", "Identity");
            #endregion
        }
    }
}