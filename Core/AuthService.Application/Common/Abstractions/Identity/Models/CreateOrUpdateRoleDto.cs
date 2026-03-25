/**
 * CreateOrUpdateRoleDto contains data for creating or updating a role.
 *
 * <p>Used by IIdentityRoleService.CreateOrUpdateAsync.</p>
 */
namespace AuthService.Application.Common.Abstractions.Identity.Models;


/// <summary>
/// Data for creating or updating a role.
/// </summary>
public sealed record CreateOrUpdateRoleDto(
    Guid? Id,
    string Name,
    string? Description);
