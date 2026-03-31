namespace AuthService.Persistence;

using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using AuthService.Persistence.Settings;
using AuthService.Application.Common.ApplicationServices.Persistence;
using AuthService.Application.Common.ApplicationServices.BackgroundJob;
using AuthService.Persistence.Common;
using AuthService.Persistence.DatabaseContext;
using AuthService.Persistence.BackgroundJobs.Outbox;


public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructurePersistence(this IServiceCollection services,
        IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("DefaultConnection") ??
                                  throw new ArgumentNullException(nameof(configuration));


        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(connectionString);
        });

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ApplicationDbContext>());

        services.AddSingleton<ISqlConnectionFactory>(_ =>
            new SqlConnectionFactory(connectionString));

        services._AddOutbox(configuration);

        return services;
    }

    private static IServiceCollection _AddOutbox(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<OutboxSettings>(configuration.GetSection("OutboxSettings"));
        return services;
    }

    /// <summary>
    /// Registers recurring job to process outbox messages.
    /// </summary>
    public static void AddOutBoxJob(this IServiceProvider serviceProvider, IConfiguration configuration)
    {
        using var scope = serviceProvider.CreateScope();

        var job = scope.ServiceProvider.GetRequiredService<IJobService>();
        var settings = scope.ServiceProvider
            .GetRequiredService<IOptions<OutboxSettings>>()
            .Value;

        job.Recurring<ProcessOutboxMessagesJob>(
            "ProcessOutboxMessages",
            job => job.Execute(),
            $"*/{settings.IntervalInMinutes} * * * *");
    }
}
