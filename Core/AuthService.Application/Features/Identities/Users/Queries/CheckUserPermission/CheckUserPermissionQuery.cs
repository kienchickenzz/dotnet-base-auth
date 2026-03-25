/**
 * CheckUserPermissionQuery checks if user has a specific permission.
 *
 * <p>Processed by CheckUserPermissionQueryHandler.</p>
 */
namespace AuthService.Application.Features.Identities.Users.Queries.CheckUserPermission;

using AuthService.Application.Common.Messaging;


/// <summary>
/// Query to check user permission.
/// </summary>
/// <param name="UserId">User's unique identifier.</param>
/// <param name="Permission">Permission to check.</param>
public sealed record CheckUserPermissionQuery(
    Guid UserId,
    string Permission) : IQuery<bool>;
