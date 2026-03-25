using AuthService.Application;
using AuthService.Infrastructure;
using AuthService.Api.Configurations;
using AuthService.Api.Extensions;
using AuthService.Api.OpenApi;
using AuthService.Identity;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextAccessor();

builder.AddConfigurations();
builder.Host.UseSerilogFromSettings();

builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();
builder.Services.AddApiServices();
builder.Services.AddApplication();

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddInfrastureIdentity(builder.Configuration);

var app = builder.Build();

// Initialize database (migrate + seed)
await app.Services.InitializeDatabaseAsync();

app.UseSwaggerExtension();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.UseInfrastructure();
app.UseInfrastructureIdentity();

app.UseCustomExceptionHandler();

app.Lifetime.ApplicationStarted.Register(() =>
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    var urls = string.Join(", ", app.Urls);
    logger.LogInformation("----------");
    logger.LogInformation("Application started successfully!");
    logger.LogInformation("Environment: {Environment}", app.Environment.EnvironmentName);
    logger.LogInformation("Listening on: {Urls}", urls);
    logger.LogInformation("Swagger UI: {Urls}/swagger", app.Urls.FirstOrDefault());
    logger.LogInformation("----------");
});

app.Run();


public partial class Program { }
