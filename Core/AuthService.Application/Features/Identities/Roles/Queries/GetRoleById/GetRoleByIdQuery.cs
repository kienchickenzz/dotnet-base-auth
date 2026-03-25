/**
 * GetRoleByIdQuery retrieves a role by Id.
 *
 * <p>Processed by GetRoleByIdQueryHandler.</p>
 */
namespace AuthService.Application.Features.Identities.Roles.Queries.GetRoleById;

using AuthService.Application.Common.Abstractions.Identity.Models;
using AuthService.Application.Common.Messaging;


/// <summary>
/// Query to get role by Id.
/// </summary>
/// <param name="RoleId">Role's unique identifier.</param>
public sealed record GetRoleByIdQuery(Guid RoleId) : IQuery<RoleDto>;
