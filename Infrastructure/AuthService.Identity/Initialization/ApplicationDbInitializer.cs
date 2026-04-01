/**
 * Extension method để initialize Identity database (migrate + seed).
 *
 * <p>Follows the same pattern as Persistence.ApplicationDbInitializer.</p>
 */
namespace AuthService.Identity.Initialization;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using AuthService.Identity.DatabaseContext;


/// <summary>
/// Extension methods for Identity database initialization.
/// </summary>
public static class ApplicationDbInitializer
{
    /// <summary>
    /// Apply pending migrations và seed data cho Identity database.
    /// </summary>
    public static async Task InitializeIdentityDatabaseAsync(
        this IServiceProvider services,
        CancellationToken cancellationToken = default)
    {
        using var scope = services.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationIdentityDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationIdentityDbContext>>();

        if (!dbContext.Database.GetMigrations().Any())
        {
            return;
        }

        if ((await dbContext.Database.GetPendingMigrationsAsync(cancellationToken)).Any())
        {
            logger.LogInformation("Applying Migrations for Identity database...");
            await dbContext.Database.MigrateAsync(cancellationToken);
        }

        if (await dbContext.Database.CanConnectAsync(cancellationToken))
        {
            logger.LogInformation("Connection to Identity database succeeded.");

            var seeder = scope.ServiceProvider.GetRequiredService<ApplicationDbSeeder>();
            await seeder.SeedDatabaseAsync(dbContext, cancellationToken);
        }
    }
}
