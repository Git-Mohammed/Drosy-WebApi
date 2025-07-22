using Drosy.Api.Extensions.DependencyInjection;
using Drosy.Api.Filters;
using Drosy.Domain.Shared.ErrorComponents;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Options;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.



builder.Services.AddControllers(options =>
{
    options.Filters.Add<FluentValidationFilter>();

    var policy = new AuthorizationPolicyBuilder()
                               .RequireAuthenticatedUser()
                               .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddDBInitializer();
builder.Services.AddLocalizationServicesConfiguration();

#region Cors Configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("DrosyPolicy",
        CorsPolicyBuilder =>
        {
            CorsPolicyBuilder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
        });
});
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
        options.SwaggerEndpoint("/openapi/v1.json", "api")
    );
}
#region Localication Middleware
var options = app.Services.GetService<IOptions<RequestLocalizationOptions>>();
app.UseRequestLocalization(options.Value);
#endregion

app.UseHttpsRedirection();
app.UseRouting();

#region Localization Middleware
app.UseRequestLocalization(); // Add this middleware BEFORE any middleware that needs culture info (like MVC/Controllers)

app.Use(async (context, next) => // Add this custom middleware after UseRequestLocalization()
{
    // CultureInfo.CurrentUICulture is automatically set by UseRequestLocalization()
    // Assign it to your static Error.CurrentLanguage property
    AppError.CurrentLanguage = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
    await next();
});
#endregion

app.UseCors("DrosyPolicy");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.setupDBInitializer();



app.Run();