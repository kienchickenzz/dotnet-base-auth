/**
 * ForgotPasswordCommandHandler processes ForgotPasswordCommand.
 *
 * <p>Initiates password reset via IIdentityUserService abstraction.</p>
 */
namespace AuthService.Application.Features.Identities.Users.Commands.Password;

using AuthService.Application.Common.Abstractions.Identity;
using AuthService.Application.Common.Messaging;
using AuthService.Domain.Common;


/// <summary>
/// Handler for ForgotPasswordCommand.
/// </summary>
public sealed class ForgotPasswordCommandHandler : ICommandHandler<ForgotPasswordCommand, string>
{
    private readonly IIdentityUserService _userService;

    public ForgotPasswordCommandHandler(IIdentityUserService userService)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
    }

    public async Task<Result<string>> Handle(
        ForgotPasswordCommand request,
        CancellationToken cancellationToken)
    {
        return await _userService.ForgotPasswordAsync(
            request.Email,
            request.Origin,
            cancellationToken);
    }
}
