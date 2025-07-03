using Drosy.Application.Interfaces.Common;
using Drosy.Infrastructure.DbInitializer;

namespace Drosy.Api.Extensions.DependencyInjection
{
    public static class DbInitializerExtention
    {
        public static IServiceCollection AddDBInitializer(this IServiceCollection services)
        {
            services.AddScoped<IDbInitializer, DbInitializer>();
            return services;
        }

        public static void setupDBInitializer(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
                dbInitializer.Initialize();
            }
        }
    }
}
