/**
 * LogoutCommandHandler processes user logout requests.
 *
 * <p>Clears refresh token to invalidate session.</p>
 */
namespace AuthService.Application.Features.Identities.Authentication.Commands.Logout;

using AuthService.Application.Common.Messaging;
using AuthService.Application.Features.Identities.Authentication.Services;
using AuthService.Domain.Common;


/// <summary>
/// Handler for LogoutCommand.
/// </summary>
public sealed class LogoutCommandHandler : ICommandHandler<LogoutCommand>
{
    private readonly IAuthenticationService _authService;

    public LogoutCommandHandler(IAuthenticationService authService)
    {
        _authService = authService;
    }

    public async Task<Result> Handle(
        LogoutCommand request,
        CancellationToken cancellationToken)
    {
        await _authService.RevokeRefreshTokenAsync(request.UserId, cancellationToken);
        return Result.Success();
    }
}
