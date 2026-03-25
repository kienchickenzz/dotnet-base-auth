/**
 * GetRoleWithPermissionsQuery retrieves a role with its permissions.
 *
 * <p>Processed by GetRoleWithPermissionsQueryHandler.</p>
 */
namespace AuthService.Application.Features.Identities.Roles.Queries.GetRoleWithPermissions;

using AuthService.Application.Common.Abstractions.Identity.Models;
using AuthService.Application.Common.Messaging;


/// <summary>
/// Query to get role with permissions.
/// </summary>
/// <param name="RoleId">Role's unique identifier.</param>
public sealed record GetRoleWithPermissionsQuery(Guid RoleId) : IQuery<RoleDto>;
