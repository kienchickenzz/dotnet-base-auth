/**
 * ResetPasswordCommandHandler processes ResetPasswordCommand.
 *
 * <p>Resets password via IIdentityUserService abstraction.</p>
 */
namespace AuthService.Application.Features.Identities.Users.Commands.Password;

using AuthService.Application.Common.Abstractions.Identity;
using AuthService.Application.Common.Messaging;
using AuthService.Domain.Common;


/// <summary>
/// Handler for ResetPasswordCommand.
/// </summary>
public sealed class ResetPasswordCommandHandler : ICommandHandler<ResetPasswordCommand>
{
    private readonly IIdentityUserService _userService;

    public ResetPasswordCommandHandler(IIdentityUserService userService)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
    }

    public async Task<Result> Handle(
        ResetPasswordCommand request,
        CancellationToken cancellationToken)
    {
        return await _userService.ResetPasswordAsync(
            request.Email,
            request.Token,
            request.NewPassword,
            cancellationToken);
    }
}
