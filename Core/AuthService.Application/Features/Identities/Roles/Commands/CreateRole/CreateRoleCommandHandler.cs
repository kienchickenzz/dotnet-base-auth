/**
 * CreateRoleCommandHandler processes CreateRoleCommand.
 *
 * <p>Creates role via IIdentityRoleService abstraction.</p>
 */
namespace AuthService.Application.Features.Identities.Roles.Commands.CreateRole;

using AuthService.Application.Common.Abstractions.Identity;
using AuthService.Application.Common.Abstractions.Identity.Models;
using AuthService.Application.Common.Messaging;
using AuthService.Domain.Common;


/// <summary>
/// Handler for CreateRoleCommand.
/// </summary>
public sealed class CreateRoleCommandHandler : ICommandHandler<CreateRoleCommand, Guid>
{
    private readonly IIdentityRoleService _roleService;

    public CreateRoleCommandHandler(IIdentityRoleService roleService)
    {
        _roleService = roleService ?? throw new ArgumentNullException(nameof(roleService));
    }

    public async Task<Result<Guid>> Handle(
        CreateRoleCommand request,
        CancellationToken cancellationToken)
    {
        var createRoleDto = new CreateOrUpdateRoleDto(
            null,
            request.Name,
            request.Description);

        return await _roleService.CreateOrUpdateAsync(createRoleDto, cancellationToken);
    }
}
