/**
 * GetRoleByIdQueryHandler processes GetRoleByIdQuery.
 *
 * <p>Retrieves role by Id via IIdentityRoleService abstraction.</p>
 */
namespace AuthService.Application.Features.Identities.Roles.Queries.GetRoleById;

using AuthService.Application.Common.Abstractions.Identity;
using AuthService.Application.Common.Abstractions.Identity.Models;
using AuthService.Application.Common.Messaging;
using AuthService.Domain.Common;


/// <summary>
/// Handler for GetRoleByIdQuery.
/// </summary>
public sealed class GetRoleByIdQueryHandler : IQueryHandler<GetRoleByIdQuery, RoleDto>
{
    private readonly IIdentityRoleService _roleService;

    public GetRoleByIdQueryHandler(IIdentityRoleService roleService)
    {
        _roleService = roleService ?? throw new ArgumentNullException(nameof(roleService));
    }

    public async Task<Result<RoleDto>> Handle(
        GetRoleByIdQuery request,
        CancellationToken cancellationToken)
    {
        return await _roleService.GetByIdAsync(request.RoleId, cancellationToken);
    }
}
