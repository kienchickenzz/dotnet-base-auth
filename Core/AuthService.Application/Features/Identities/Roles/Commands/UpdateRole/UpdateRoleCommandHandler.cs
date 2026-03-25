/**
 * UpdateRoleCommandHandler processes UpdateRoleCommand.
 *
 * <p>Updates role via IIdentityRoleService abstraction.</p>
 */
namespace AuthService.Application.Features.Identities.Roles.Commands.UpdateRole;

using AuthService.Application.Common.Abstractions.Identity;
using AuthService.Application.Common.Abstractions.Identity.Models;
using AuthService.Application.Common.Messaging;
using AuthService.Domain.Common;


/// <summary>
/// Handler for UpdateRoleCommand.
/// </summary>
public sealed class UpdateRoleCommandHandler : ICommandHandler<UpdateRoleCommand, Guid>
{
    private readonly IIdentityRoleService _roleService;

    public UpdateRoleCommandHandler(IIdentityRoleService roleService)
    {
        _roleService = roleService ?? throw new ArgumentNullException(nameof(roleService));
    }

    public async Task<Result<Guid>> Handle(
        UpdateRoleCommand request,
        CancellationToken cancellationToken)
    {
        var updateRoleDto = new CreateOrUpdateRoleDto(
            request.Id,
            request.Name,
            request.Description);

        return await _roleService.CreateOrUpdateAsync(updateRoleDto, cancellationToken);
    }
}
