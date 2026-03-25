/**
 * CheckUserPermissionQueryHandler processes CheckUserPermissionQuery.
 *
 * <p>Checks user permission via IIdentityPermissionService abstraction.</p>
 */
namespace AuthService.Application.Features.Identities.Users.Queries.CheckUserPermission;

using AuthService.Application.Common.Abstractions.Identity;
using AuthService.Application.Common.Messaging;
using AuthService.Domain.Common;


/// <summary>
/// Handler for CheckUserPermissionQuery.
/// </summary>
public sealed class CheckUserPermissionQueryHandler : IQueryHandler<CheckUserPermissionQuery, bool>
{
    private readonly IIdentityPermissionService _permissionService;

    public CheckUserPermissionQueryHandler(IIdentityPermissionService permissionService)
    {
        _permissionService = permissionService ?? throw new ArgumentNullException(nameof(permissionService));
    }

    public async Task<Result<bool>> Handle(
        CheckUserPermissionQuery request,
        CancellationToken cancellationToken)
    {
        var hasPermission = await _permissionService.HasPermissionAsync(
            request.UserId,
            request.Permission,
            cancellationToken);

        return Result.Success(hasPermission);
    }
}
