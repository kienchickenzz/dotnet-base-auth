/**
 * GetUserPermissionsQuery retrieves permissions for a user.
 *
 * <p>Processed by GetUserPermissionsQueryHandler.</p>
 */
namespace AuthService.Application.Features.Identities.Users.Queries.GetUserPermissions;

using AuthService.Application.Common.Messaging;


/// <summary>
/// Query to get user permissions.
/// </summary>
/// <param name="UserId">User's unique identifier.</param>
public sealed record GetUserPermissionsQuery(Guid UserId) : IQuery<List<string>>;
