/**
 * AuthController handles authentication API endpoints.
 *
 * <p>Provides login, refresh token and logout functionality.</p>
 */
namespace AuthService.Api.Controllers.Identity;

using MediatR;

using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using AuthService.Application.Common.Extensions.Identity;
using AuthService.Application.Features.Identities.Authentication;
using AuthService.Application.Features.Identities.Authentication.Commands.Logout;


/// <summary>
/// Authentication endpoints.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public sealed class AuthController : ControllerBase
{
    private readonly ITokenService _tokenService;
    private readonly ISender _sender;

    public AuthController(ITokenService tokenService, ISender sender)
    {
        _tokenService = tokenService;
        _sender = sender;
    }

    /// <summary>
    /// Login.
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public Task<LoginResponse> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        return _tokenService.GetTokenAsync(request, _GetIpAddress()!, cancellationToken);
    }

    /// <summary>
    /// Refresh token.
    /// </summary>
    [HttpPost("refresh")]
    [AllowAnonymous]
    public Task<LoginResponse> RefreshAsync(RefreshTokenRequest request)
    {
        return _tokenService.RefreshTokenAsync(request, _GetIpAddress()!);
    }

    /// <summary>
    /// Logout current user and revoke refresh token.
    /// </summary>
    [HttpPost("logout")]
    [Authorize]
    public async Task<ActionResult> LogoutAsync(CancellationToken cancellationToken)
    {
        var userIdString = User.GetUserId();
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
        {
            return Unauthorized();
        }

        var result = await _sender.Send(new LogoutCommand(userId), cancellationToken);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    private string? _GetIpAddress() =>
        Request.Headers.ContainsKey("X-Forwarded-For")
            ? Request.Headers["X-Forwarded-For"]
            : HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString() ?? "N/A";
}
