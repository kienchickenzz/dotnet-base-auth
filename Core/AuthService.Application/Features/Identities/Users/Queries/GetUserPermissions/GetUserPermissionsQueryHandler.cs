/**
 * GetUserPermissionsQueryHandler processes GetUserPermissionsQuery.
 *
 * <p>Retrieves user permissions via IIdentityPermissionService abstraction.</p>
 */
namespace AuthService.Application.Features.Identities.Users.Queries.GetUserPermissions;

using AuthService.Application.Common.Abstractions.Identity;
using AuthService.Application.Common.Messaging;
using AuthService.Domain.Common;


/// <summary>
/// Handler for GetUserPermissionsQuery.
/// </summary>
public sealed class GetUserPermissionsQueryHandler : IQueryHandler<GetUserPermissionsQuery, List<string>>
{
    private readonly IIdentityPermissionService _permissionService;

    public GetUserPermissionsQueryHandler(IIdentityPermissionService permissionService)
    {
        _permissionService = permissionService ?? throw new ArgumentNullException(nameof(permissionService));
    }

    public async Task<Result<List<string>>> Handle(
        GetUserPermissionsQuery request,
        CancellationToken cancellationToken)
    {
        return await _permissionService.GetPermissionsAsync(request.UserId, cancellationToken);
    }
}
