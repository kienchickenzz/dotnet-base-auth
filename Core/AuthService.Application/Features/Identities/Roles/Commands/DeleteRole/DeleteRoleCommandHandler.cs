/**
 * DeleteRoleCommandHandler processes DeleteRoleCommand.
 *
 * <p>Deletes role via IIdentityRoleService abstraction.</p>
 */
namespace AuthService.Application.Features.Identities.Roles.Commands.DeleteRole;

using AuthService.Application.Common.Abstractions.Identity;
using AuthService.Application.Common.Messaging;
using AuthService.Domain.Common;


/// <summary>
/// Handler for DeleteRoleCommand.
/// </summary>
public sealed class DeleteRoleCommandHandler : ICommandHandler<DeleteRoleCommand>
{
    private readonly IIdentityRoleService _roleService;

    public DeleteRoleCommandHandler(IIdentityRoleService roleService)
    {
        _roleService = roleService ?? throw new ArgumentNullException(nameof(roleService));
    }

    public async Task<Result> Handle(
        DeleteRoleCommand request,
        CancellationToken cancellationToken)
    {
        return await _roleService.DeleteAsync(request.RoleId, cancellationToken);
    }
}
