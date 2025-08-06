using System.Reflection.Emit;
using Drosy.Application.UseCases.Dashboard.DTOs;
using Drosy.Domain.Entities;
using Drosy.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Drosy.Infrastructure.Persistence.DbContexts
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : IdentityDbContext<ApplicationUser,ApplicationRole,int>(options)
    {

        public DbSet<ApplicationUser> AppUsers { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<PlanStudent> PlanStudents { get; set; }
        public DbSet<Plan> Plans { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Assistant> Assistants { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Grade> Grades { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Attendence> Attendences { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<DashboardStatsViewDTO> DashboardStats {  get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<SystemSetting> SystemSettings { get; set; }
        public DbSet<Region> Regions { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
            builder.Ignore<AppUser>();
           
            builder
               .Entity<DashboardStatsViewDTO>()
               .ToView("vw_DashboardStats")
               .HasNoKey();

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