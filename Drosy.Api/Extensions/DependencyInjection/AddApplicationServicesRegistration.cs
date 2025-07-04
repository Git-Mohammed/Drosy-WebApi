﻿using System.Text;
using Drosy.Application.Features.Authentication.DTOs;
using Drosy.Application.Interfaces;
using Drosy.Application.Interfaces.Common;
using Drosy.Infrastructure.Identity;
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