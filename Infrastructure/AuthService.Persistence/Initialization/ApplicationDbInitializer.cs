// -----------------------------------------------------------------------------
// <summary>
//     Extension method để initialize database (migrate + seed).
// </summary>
// -----------------------------------------------------------------------------
namespace AuthService.Persistence.Initialization;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using AuthService.Persistence.DatabaseContext;


public static class ApplicationDbInitializer
{
    /// <summary>
    /// Apply pending migrations và seed data.
    /// </summary>
    public static async Task InitializeApplicationDatabaseAsync(this IServiceProvider services, CancellationToken cancellationToken = default)
    {
        using var scope = services.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();

        if (!dbContext.Database.GetMigrations().Any())
        {
            return;
        }

        if ((await dbContext.Database.GetPendingMigrationsAsync(cancellationToken)).Any())
        {
            logger.LogInformation("Applying Migrations for database...");
            await dbContext.Database.MigrateAsync(cancellationToken);
        }
    }
}
