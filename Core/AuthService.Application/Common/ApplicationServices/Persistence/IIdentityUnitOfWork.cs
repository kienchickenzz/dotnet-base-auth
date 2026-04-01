/**
 * IIdentityUnitOfWork defines the contract for Identity database operations.
 *
 * <p>This interface enables centralized transaction management for Identity context.</p>
 */
namespace AuthService.Application.Common.ApplicationServices.Persistence;


/// <summary>
/// Unit of Work interface for Identity database context.
/// </summary>
public interface IIdentityUnitOfWork : IAsyncDisposable
{
    /// <summary>
    /// Saves all pending changes to the Identity database.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Number of affected rows.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
