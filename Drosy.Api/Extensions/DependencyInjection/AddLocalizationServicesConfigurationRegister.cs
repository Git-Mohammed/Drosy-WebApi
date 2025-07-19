using Microsoft.AspNetCore.Localization;
using System.Globalization;

namespace Drosy.Api.Extensions.DependencyInjection
{
    public static class AddLocalizationServicesConfigurationRegister
    {
        public static IServiceCollection AddLocalizationServicesConfiguration(this IServiceCollection services)
        {
            #region Localization Services Configuration
            services.AddLocalization(opt =>
                opt.ResourcesPath = "Resoureces"

            ); // Add localization services

            services.Configure<RequestLocalizationOptions>(options => // Configure supported cultures
            {
                var supportedCultures = new[]
                {
                    new CultureInfo("en"), // English
                    new CultureInfo("ar"), // Arabic
                    // Add all other supported cultures here (e.g., new CultureInfo("pt"))
                };

                options.DefaultRequestCulture = new RequestCulture("ar"); // Set your default culture
                options.SupportedCultures = supportedCultures; // Specify supported cultures
                options.SupportedUICultures = supportedCultures; // Specify supported UI cultures

                // Use AcceptLanguageHeaderRequestCultureProvider for APIs, which is often the default.
                options.RequestCultureProviders.Insert(0, new AcceptLanguageHeaderRequestCultureProvider());
            });

            return services;
        }
        #endregion
    }

}
