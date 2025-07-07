
using System.Text;
using Drosy.Application.UsesCases.Authentication.DTOs;
using Drosy.Application.Interfaces;
using Drosy.Application.Interfaces.Common;
using Drosy.Infrastructure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Drosy.Application.UsesCases.Authentication.Interfaces;
using Drosy.Application.UsesCases.Authentication.Services;
using Drosy.Domain.Interfaces.Repository;
using Drosy.Infrastructure.Persistence.Repositories;
using Drosy.Application.UseCases.Students.Interfaces;
using Drosy.Application.UseCases.Students.Services;

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
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authOption.SigningKey))
                    };
                });

            #endregion

            return services;
        }
    }
}