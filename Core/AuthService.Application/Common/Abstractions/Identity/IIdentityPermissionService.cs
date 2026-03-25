/**
 * IIdentityPermissionService defines contract for permission operations.
 *
 * <p>Handles permission queries and caching.</p>
 */
namespace AuthService.Application.Common.Abstractions.Identity;

using AuthService.Domain.Common;


/// <summary>
/// Service for permission operations.
/// </summary>
public interface IIdentityPermissionService
{
    /// <summary>
    /// Gets all permissions for a user.
    /// </summary>
    Task<Result<List<string>>> GetPermissionsAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if user has a specific permission.
    /// </summary>
    Task<bool> HasPermissionAsync(
        Guid userId,
        string permission,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Invalidates permission cache for a user.
    /// </summary>
    Task InvalidateCacheAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
}
