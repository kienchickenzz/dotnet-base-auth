/**
 * UpdateRolePermissionsCommandHandler processes UpdateRolePermissionsCommand.
 *
 * <p>Updates role permissions via IIdentityRoleService abstraction.</p>
 */
namespace AuthService.Application.Features.Identities.Roles.Commands.UpdateRolePermissions;

using AuthService.Application.Common.Abstractions.Identity;
using AuthService.Application.Common.Messaging;
using AuthService.Domain.Common;


/// <summary>
/// Handler for UpdateRolePermissionsCommand.
/// </summary>
public sealed class UpdateRolePermissionsCommandHandler : ICommandHandler<UpdateRolePermissionsCommand>
{
    private readonly IIdentityRoleService _roleService;

    public UpdateRolePermissionsCommandHandler(IIdentityRoleService roleService)
    {
        _roleService = roleService ?? throw new ArgumentNullException(nameof(roleService));
    }

    public async Task<Result> Handle(
        UpdateRolePermissionsCommand request,
        CancellationToken cancellationToken)
    {
        return await _roleService.UpdatePermissionsAsync(
            request.RoleId,
            request.Permissions,
            cancellationToken);
    }
}
