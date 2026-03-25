/**
 * ConfirmEmailCommandHandler processes ConfirmEmailCommand.
 *
 * <p>Confirms email via IIdentityUserService abstraction.</p>
 */
namespace AuthService.Application.Features.Identities.Users.Commands.Confirm;

using AuthService.Application.Common.Abstractions.Identity;
using AuthService.Application.Common.Messaging;
using AuthService.Domain.Common;


/// <summary>
/// Handler for ConfirmEmailCommand.
/// </summary>
public sealed class ConfirmEmailCommandHandler : ICommandHandler<ConfirmEmailCommand, string>
{
    private readonly IIdentityUserService _userService;

    public ConfirmEmailCommandHandler(IIdentityUserService userService)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
    }

    public async Task<Result<string>> Handle(
        ConfirmEmailCommand request,
        CancellationToken cancellationToken)
    {
        return await _userService.ConfirmEmailAsync(
            request.UserId,
            request.Code,
            cancellationToken);
    }
}
