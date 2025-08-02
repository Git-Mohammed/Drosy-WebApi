using Drosy.Application.Interfaces.Common;
using Drosy.Domain.Interfaces.Common.Uow;
using Drosy.Domain.Interfaces.Repository;
using Drosy.Infrastructure.Identity.Entities;
using Drosy.Infrastructure.Logging;
using Drosy.Infrastructure.Mapping.Configs;
using Drosy.Infrastructure.Persistence.DbContexts;
using Drosy.Infrastructure.Persistence.Intercepters;
using Drosy.Infrastructure.Persistence.Repositories;
using Drosy.Infrastructure.Persistence.Uow;
using Drosy.Infrastructure.Validators;
using FluentValidation;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Drosy.Api.Extensions.DependencyInjection
{
    public static class AddInfrastructureServicesRegistrationExtension
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            #region Register fluentValidators
            services.AddValidatorsFromAssemblyContaining<SampleValidator>();
            #endregion

            #region Register Mapper:Mapster
            MappingConfig.Configure();
            services.AddSingleton(TypeAdapterConfig.GlobalSettings);
            services.AddScoped<MapsterMapper.IMapper, MapsterMapper.ServiceMapper>();
            services.AddScoped<IMapper, MappingServiceAdapter>();
            #endregion

            #region Register Logger 
            services.AddScoped(typeof(Application.Interfaces.Common.ILogger<>), typeof(LoggerAdapter<>));
            #endregion

            #region Register Identity

            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = false;
                options.User.RequireUniqueEmail = true;
            }).AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
            #endregion

            #region  Interceptors
            services.AddScoped<AuditAtInterceptor>();
            #endregion
            
            #region Register Ef Core
            services.AddDbContext<ApplicationDbContext>(option =>
            {
                option.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
                    .AddInterceptors(new AuditAtInterceptor());
            });
            #endregion

            #region Custom Repostiories
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IStudentRepository, StudentRepository>();
            services.AddScoped<IPlanStudentsRepository, PlanStudentsRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IAppUserRepository, AppUserRepository>();
            services.AddScoped<IPlanRepository, PlanRepository>();
            services.AddScoped<ISessionRepository, SessionRepository>();
            services.AddScoped<IPasswordResetTokenRepository, PasswordResetTokenRepository>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<IAttendencesRepository, AttendencesRepository>();

            #endregion

            return services;
        }
    }
}