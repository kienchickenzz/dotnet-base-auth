/**
 * IIdentityRoleService defines contract for role management operations.
 *
 * <p>Abstracts role operations from identity framework implementation.</p>
 */
namespace AuthService.Application.Common.Abstractions.Identity;

using AuthService.Application.Common.Abstractions.Identity.Models;
using AuthService.Domain.Common;


/// <summary>
/// Service for role management operations.
/// </summary>
public interface IIdentityRoleService
{
    /// <summary>
    /// Gets all roles.
    /// </summary>
    Task<Result<List<RoleDto>>> GetRolesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets role by Id.
    /// </summary>
    Task<Result<RoleDto>> GetByIdAsync(Guid roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets role by Id with permissions.
    /// </summary>
    Task<Result<RoleDto>> GetByIdWithPermissionsAsync(
        Guid roleId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets role count.
    /// </summary>
    Task<int> GetCountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if role exists.
    /// </summary>
    Task<bool> ExistsAsync(string roleName, Guid? excludeId = null);

    /// <summary>
    /// Creates or updates a role.
    /// </summary>
    Task<Result<Guid>> CreateOrUpdateAsync(
        CreateOrUpdateRoleDto role,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a role.
    /// </summary>
    Task<Result> DeleteAsync(Guid roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates role permissions.
    /// </summary>
    Task<Result> UpdatePermissionsAsync(
        Guid roleId,
        List<string> permissions,
        CancellationToken cancellationToken = default);
}
