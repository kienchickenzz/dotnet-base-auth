/**
 * ConfirmPhoneNumberCommandHandler processes ConfirmPhoneNumberCommand.
 *
 * <p>Confirms phone number via IIdentityUserService abstraction.</p>
 */
namespace AuthService.Application.Features.Identities.Users.Commands.Confirm;

using AuthService.Application.Common.Abstractions.Identity;
using AuthService.Application.Common.Messaging;
using AuthService.Domain.Common;


/// <summary>
/// Handler for ConfirmPhoneNumberCommand.
/// </summary>
public sealed class ConfirmPhoneNumberCommandHandler : ICommandHandler<ConfirmPhoneNumberCommand, string>
{
    private readonly IIdentityUserService _userService;

    public ConfirmPhoneNumberCommandHandler(IIdentityUserService userService)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
    }

    public async Task<Result<string>> Handle(
        ConfirmPhoneNumberCommand request,
        CancellationToken cancellationToken)
    {
        return await _userService.ConfirmPhoneNumberAsync(
            request.UserId,
            request.Code,
            cancellationToken);
    }
}
