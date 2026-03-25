/**
 * GetRoleWithPermissionsQueryHandler processes GetRoleWithPermissionsQuery.
 *
 * <p>Retrieves role with permissions via IIdentityRoleService abstraction.</p>
 */
namespace AuthService.Application.Features.Identities.Roles.Queries.GetRoleWithPermissions;

using AuthService.Application.Common.Abstractions.Identity;
using AuthService.Application.Common.Abstractions.Identity.Models;
using AuthService.Application.Common.Messaging;
using AuthService.Domain.Common;


/// <summary>
/// Handler for GetRoleWithPermissionsQuery.
/// </summary>
public sealed class GetRoleWithPermissionsQueryHandler : IQueryHandler<GetRoleWithPermissionsQuery, RoleDto>
{
    private readonly IIdentityRoleService _roleService;

    public GetRoleWithPermissionsQueryHandler(IIdentityRoleService roleService)
    {
        _roleService = roleService ?? throw new ArgumentNullException(nameof(roleService));
    }

    public async Task<Result<RoleDto>> Handle(
        GetRoleWithPermissionsQuery request,
        CancellationToken cancellationToken)
    {
        return await _roleService.GetByIdWithPermissionsAsync(request.RoleId, cancellationToken);
    }
}
