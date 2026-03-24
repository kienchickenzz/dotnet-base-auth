using AuthService.Application;
// using AuthService.Persistence;
// using BaseCleanArchitecture.Persistence.Initialization;
using AuthService.Api.Configurations;
using AuthService.Api.Extensions;
using AuthService.Api.OpenApi;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddHttpContextAccessor();

builder.AddConfigurations();
builder.Host.UseSerilogFromSettings();

builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();
builder.Services.AddApiServices();
builder.Services.AddApplication();

// builder.Services.AddInfrastructurePersistence(builder.Configuration);

var app = builder.Build();

// Initialize database (migrate + seed)
// await app.Services.InitializeDatabaseAsync();

// if (app.Environment.IsDevelopment())
// {
//     app.UseSwaggerExtension();
// }

app.UseSwaggerExtension();

app.UseRouting();
// app.UseHttpsRedirection(); // Disable for dev

app.UseAuthorization();
app.MapControllers();

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
