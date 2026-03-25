/**
 * ToggleUserStatusCommandHandler processes ToggleUserStatusCommand.
 *
 * <p>Toggles user status via IIdentityUserService abstraction.</p>
 */
namespace AuthService.Application.Features.Identities.Users.Commands.ToggleUserStatus;

using AuthService.Application.Common.Abstractions.Identity;
using AuthService.Application.Common.Messaging;
using AuthService.Domain.Common;


/// <summary>
/// Handler for ToggleUserStatusCommand.
/// </summary>
public sealed class ToggleUserStatusCommandHandler : ICommandHandler<ToggleUserStatusCommand>
{
    private readonly IIdentityUserService _userService;

    public ToggleUserStatusCommandHandler(IIdentityUserService userService)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
    }

    public async Task<Result> Handle(
        ToggleUserStatusCommand request,
        CancellationToken cancellationToken)
    {
        return await _userService.ToggleStatusAsync(
            request.UserId,
            request.Activate,
            cancellationToken);
    }
}
