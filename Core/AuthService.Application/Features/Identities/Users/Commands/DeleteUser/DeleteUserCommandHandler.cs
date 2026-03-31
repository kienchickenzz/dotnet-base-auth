/**
 * DeleteUserCommandHandler processes user deletion requests.
 *
 * <p>Performs soft delete using ISoftDelete pattern.</p>
 */
namespace AuthService.Application.Features.Identities.Users.Commands.DeleteUser;

using AuthService.Application.Common.Abstractions.Identity;
using AuthService.Application.Common.Messaging;
using AuthService.Domain.Common;


/// <summary>
/// Handler for DeleteUserCommand.
/// </summary>
public sealed class DeleteUserCommandHandler : ICommandHandler<DeleteUserCommand>
{
    private readonly IIdentityUserService _userService;

    public DeleteUserCommandHandler(IIdentityUserService userService)
    {
        _userService = userService;
    }

    public async Task<Result> Handle(
        DeleteUserCommand request,
        CancellationToken cancellationToken)
    {
        return await _userService.DeleteAsync(request.UserId, cancellationToken);
    }
}
