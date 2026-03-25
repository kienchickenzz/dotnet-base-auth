/**
 * GetRolesQueryHandler processes GetRolesQuery.
 *
 * <p>Retrieves all roles via IIdentityRoleService abstraction.</p>
 */
namespace AuthService.Application.Features.Identities.Roles.Queries.GetRoles;

using AuthService.Application.Common.Abstractions.Identity;
using AuthService.Application.Common.Abstractions.Identity.Models;
using AuthService.Application.Common.Messaging;
using AuthService.Domain.Common;


/// <summary>
/// Handler for GetRolesQuery.
/// </summary>
public sealed class GetRolesQueryHandler : IQueryHandler<GetRolesQuery, List<RoleDto>>
{
    private readonly IIdentityRoleService _roleService;

    public GetRolesQueryHandler(IIdentityRoleService roleService)
    {
        _roleService = roleService ?? throw new ArgumentNullException(nameof(roleService));
    }

    public async Task<Result<List<RoleDto>>> Handle(
        GetRolesQuery request,
        CancellationToken cancellationToken)
    {
        return await _roleService.GetRolesAsync(cancellationToken);
    }
}
