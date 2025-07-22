
using System.Text;
using Drosy.Application.Interfaces.Common;
using Drosy.Application.UseCases.Authentication.Interfaces;
using Drosy.Application.UseCases.Authentication.Services;
using Drosy.Application.UseCases.Email.DTOs;
using Drosy.Application.UseCases.Email.Interfaces;
using Drosy.Application.UseCases.Plans.Interfaces;
using Drosy.Application.UseCases.Plans.Services;
using Drosy.Application.UseCases.PlanStudents.Interfaces;
using Drosy.Application.UseCases.PlanStudents.Services;
using Drosy.Application.UseCases.Students.Interfaces;
using Drosy.Application.UseCases.Students.Services;
using Drosy.Application.UsesCases.Authentication.DTOs;
using Drosy.Infrastructure.Email.Mailkit;
using Drosy.Infrastructure.Identity;
using Drosy.Infrastructure.JWT;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Drosy.Api.Extensions.DependencyInjection
{
    public static class AddApplicationServicesRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            #region UserServices Registration
            services.AddScoped<IIdentityService, IdentityService>();
            #endregion
            
            #region Custom Services
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IStudentService, StudentService>();
            services.AddScoped<IPlanStudentsService, PlanStudentsService>();
            services.AddScoped<IPlanService, PlanService>();
            services.AddScoped<IEmailService, EmailService>();
            #endregion

            #region Email Configurations
            services.Configure<EmailOptions>(
            configuration.GetSection("EmailSettings"));
            #endregion

            #region JWT Registration


            var authOption = configuration.GetSection("JWT").Get<AuthOptions>();
            services.AddSingleton(authOption);
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = authOption.Issuer,
                        ValidAudience = authOption.Audience,
                        ClockSkew = TimeSpan.Zero,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authOption.SigningKey))
                    };
                });

            #endregion

            return services;
        }
    }
}