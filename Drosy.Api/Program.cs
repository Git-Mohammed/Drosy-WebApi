using Drosy.Api.Extensions.DependencyInjection;
using Drosy.Api.Filters;
using Drosy.Application.Interfaces.Common;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Drosy.Domain.Shared.ResultPattern.ErrorComponents;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

#region Localization Services Configuration
builder.Services.AddLocalization(); // Add localization services

builder.Services.Configure<RequestLocalizationOptions>(options => // Configure supported cultures
{
    var supportedCultures = new[]
    {
        new CultureInfo("en"), // English
        new CultureInfo("ar"), // Arabic
        // Add all other supported cultures here (e.g., new CultureInfo("pt"))
    };

    options.DefaultRequestCulture = new RequestCulture("en"); // Set your default culture
    options.SupportedCultures = supportedCultures; // Specify supported cultures
    options.SupportedUICultures = supportedCultures; // Specify supported UI cultures

    // Use AcceptLanguageHeaderRequestCultureProvider for APIs, which is often the default.
    options.RequestCultureProviders.Insert(0, new AcceptLanguageHeaderRequestCultureProvider());
});
#endregion

builder.Services.AddControllers(options =>
{
    options.Filters.Add<FluentValidationFilter>();
});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddDBInitializer();

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

app.UseHttpsRedirection();
app.UseRouting();

#region Localization Middleware
app.UseRequestLocalization(); // Add this middleware BEFORE any middleware that needs culture info (like MVC/Controllers)

app.Use(async (context, next) => // Add this custom middleware after UseRequestLocalization()
{
    // CultureInfo.CurrentUICulture is automatically set by UseRequestLocalization()
    // Assign it to your static Error.CurrentLanguage property
    Error.CurrentLanguage = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
    await next();
});
#endregion

app.UseCors("DrosyPolicy");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.setupDBInitializer();



app.Run();