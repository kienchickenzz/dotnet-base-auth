/**
 * GetUserRolesQueryHandler processes GetUserRolesQuery.
 *
 * <p>Retrieves user roles via IIdentityUserService abstraction.</p>
 */
namespace AuthService.Application.Features.Identities.Users.Queries.GetUserRoles;

using AuthService.Application.Common.Abstractions.Identity;
using AuthService.Application.Common.Abstractions.Identity.Models;
using AuthService.Application.Common.Messaging;
using AuthService.Domain.Common;


/// <summary>
/// Handler for GetUserRolesQuery.
/// </summary>
public sealed class GetUserRolesQueryHandler : IQueryHandler<GetUserRolesQuery, List<UserRoleDto>>
{
    private readonly IIdentityUserService _userService;

    public GetUserRolesQueryHandler(IIdentityUserService userService)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
    }

    public async Task<Result<List<UserRoleDto>>> Handle(
        GetUserRolesQuery request,
        CancellationToken cancellationToken)
    {
        return await _userService.GetRolesAsync(request.UserId, cancellationToken);
    }
}
