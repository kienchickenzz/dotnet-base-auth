/**
 * CreateUserCommandHandler processes CreateUserCommand.
 *
 * <p>Creates user via IIdentityUserService abstraction.</p>
 */
namespace AuthService.Application.Features.Identities.Users.Commands.CreateUser;

using AuthService.Application.Common.Abstractions.Identity;
using AuthService.Application.Common.Abstractions.Identity.Models;
using AuthService.Application.Common.Messaging;
using AuthService.Domain.Common;


/// <summary>
/// Handler for CreateUserCommand.
/// </summary>
public sealed class CreateUserCommandHandler : ICommandHandler<CreateUserCommand, Guid>
{
    private readonly IIdentityUserService _userService;

    public CreateUserCommandHandler(IIdentityUserService userService)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
    }

    public async Task<Result<Guid>> Handle(
        CreateUserCommand request,
        CancellationToken cancellationToken)
    {
        var createUserDto = new CreateUserDto(
            request.FirstName,
            request.LastName,
            request.Email,
            request.UserName,
            request.Password,
            request.PhoneNumber);

        return await _userService.CreateAsync(createUserDto, cancellationToken);
    }
}
