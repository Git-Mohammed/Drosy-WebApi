using Drosy.Api.Extensions.DependencyInjection;
using Drosy.Api.Filters;
using Drosy.Application.Interfaces.Common;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

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
app.UseCors("DrosyPolicy");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.setupDBInitializer();



app.Run();