/**
 * GetUserRolesQuery retrieves roles for a user.
 *
 * <p>Processed by GetUserRolesQueryHandler.</p>
 */
namespace AuthService.Application.Features.Identities.Users.Queries.GetUserRoles;

using AuthService.Application.Common.Abstractions.Identity.Models;
using AuthService.Application.Common.Messaging;


/// <summary>
/// Query to get user roles.
/// </summary>
/// <param name="UserId">User's unique identifier.</param>
public sealed record GetUserRolesQuery(Guid UserId) : IQuery<List<UserRoleDto>>;
