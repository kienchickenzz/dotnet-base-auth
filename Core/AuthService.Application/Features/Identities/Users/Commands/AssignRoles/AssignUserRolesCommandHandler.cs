/**
 * AssignUserRolesCommandHandler processes AssignUserRolesCommand.
 *
 * <p>Assigns roles via IIdentityUserService abstraction.</p>
 */
namespace AuthService.Application.Features.Identities.Users.Commands.AssignRoles;

using AuthService.Application.Common.Abstractions.Identity;
using AuthService.Application.Common.Messaging;
using AuthService.Domain.Common;


/// <summary>
/// Handler for AssignUserRolesCommand.
/// </summary>
public sealed class AssignUserRolesCommandHandler : ICommandHandler<AssignUserRolesCommand>
{
    private readonly IIdentityUserService _userService;

    public AssignUserRolesCommandHandler(IIdentityUserService userService)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
    }

    public async Task<Result> Handle(
        AssignUserRolesCommand request,
        CancellationToken cancellationToken)
    {
        return await _userService.AssignRolesAsync(
            request.UserId,
            request.Roles,
            cancellationToken);
    }
}
