/**
 * GetRolesQuery retrieves all roles.
 *
 * <p>Processed by GetRolesQueryHandler.</p>
 */
namespace AuthService.Application.Features.Identities.Roles.Queries.GetRoles;

using AuthService.Application.Common.Abstractions.Identity.Models;
using AuthService.Application.Common.Messaging;


/// <summary>
/// Query to get all roles.
/// </summary>
public sealed record GetRolesQuery : IQuery<List<RoleDto>>;
