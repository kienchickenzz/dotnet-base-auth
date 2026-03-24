namespace AuthService.Identity;

using AuthService.Identity.Initialization;

using Microsoft.Extensions.DependencyInjection;


public static class DependencyInjection
{
    public static async Task InitializeDatabasesAsync(this IServiceProvider services, CancellationToken cancellationToken = default)
    {
        // Create a new scope to retrieve scoped services
        using var scope = services.CreateScope();

        await scope.ServiceProvider.GetRequiredService<ApplicationDbInitializer>()
            .InitializeAsync(cancellationToken);
    }
}
