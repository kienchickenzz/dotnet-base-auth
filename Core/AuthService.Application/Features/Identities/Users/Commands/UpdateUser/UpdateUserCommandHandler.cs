/**
 * UpdateUserCommandHandler processes UpdateUserCommand.
 *
 * <p>Updates user via IIdentityUserService abstraction.</p>
 */
namespace AuthService.Application.Features.Identities.Users.Commands.UpdateUser;

using AuthService.Application.Common.Abstractions.Identity;
using AuthService.Application.Common.Abstractions.Identity.Models;
using AuthService.Application.Common.Messaging;
using AuthService.Domain.Common;


/// <summary>
/// Handler for UpdateUserCommand.
/// </summary>
public sealed class UpdateUserCommandHandler : ICommandHandler<UpdateUserCommand>
{
    private readonly IIdentityUserService _userService;

    public UpdateUserCommandHandler(IIdentityUserService userService)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
    }

    public async Task<Result> Handle(
        UpdateUserCommand request,
        CancellationToken cancellationToken)
    {
        var updateUserDto = new UpdateUserDto(
            request.Id,
            request.FirstName,
            request.LastName,
            request.Email,
            request.PhoneNumber);

        return await _userService.UpdateAsync(updateUserDto, cancellationToken);
    }
}
