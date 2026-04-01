/**
 * AccountController handles public account-related API endpoints.
 *
 * <p>Provides registration, authentication, and password recovery functionality.</p>
 */
namespace AuthService.Api.Controllers.Identity;

using MediatR;

using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using AuthService.Api.Contracts.Identity;
using AuthService.Api.Extensions;
using AuthService.Application.Common.Extensions.Identity;
using AuthService.Application.Features.Identities.Authentication;
using AuthService.Application.Features.Identities.Authentication.Commands.Login;
using AuthService.Application.Features.Identities.Authentication.Commands.Logout;
using AuthService.Application.Features.Identities.Authentication.Commands.RefreshToken;
using AuthService.Application.Features.Identities.Authentication.Models.Responses;
using AuthService.Application.Features.Identities.Users.Commands.Confirm;
using AuthService.Application.Features.Identities.Users.Commands.CreateUser;
using AuthService.Application.Features.Identities.Users.Commands.Password;


/// <summary>
/// Public account endpoints for registration, authentication and password recovery.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly ISender _sender;

    public AccountController(ISender sender)
    {
        _sender = sender ?? throw new ArgumentNullException(nameof(sender));
    }

    #region Authentication

    /// <summary>
    /// Login with email and password.
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<TokenResponse>> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken)
    {
        var command = new LoginCommand(
            request.Email,
            request.Password,
            _GetIpAddress()!);

        var result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : result.Error.ToUnauthorized();
    }

    /// <summary>
    /// Refresh access token using refresh token.
    /// </summary>
    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<ActionResult<TokenResponse>> RefreshAsync(
        RefreshTokenRequest request,
        CancellationToken cancellationToken)
    {
        var command = new RefreshTokenCommand(
            request.Token,
            request.RefreshToken,
            _GetIpAddress()!);

        var result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : result.Error.ToUnauthorized();
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
        return result.IsSuccess ? NoContent() : result.Error.ToBadRequest();
    }

    #endregion

    #region Registration

    /// <summary>
    /// Register a new user account.
    /// </summary>
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<Guid>> RegisterAsync(
        CreateUserRequest request,
        CancellationToken cancellationToken)
    {
        string origin = $"{Request.Scheme}://{Request.Host}";

        var command = new CreateUserCommand(
            request.FirstName,
            request.LastName,
            request.Email,
            request.UserName,
            request.Password,
            request.ConfirmPassword,
            request.PhoneNumber,
            origin);

        var result = await _sender.Send(command, cancellationToken);
        if (!result.IsSuccess)
        {
            return result.Error.ToBadRequest();
        }

        return Created($"/api/users/{result.Value}", result.Value);
    }

    #endregion

    #region Email & Phone Confirmation

    /// <summary>
    /// Confirms user email.
    /// </summary>
    [HttpGet("confirm-email")]
    [AllowAnonymous]
    public async Task<ActionResult<string>> ConfirmEmailAsync(
        [FromQuery] Guid userId,
        [FromQuery] string code,
        CancellationToken cancellationToken)
    {
        var command = new ConfirmEmailCommand(userId, code);
        var result = await _sender.Send(command, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.Error.ToBadRequest();
    }

    /// <summary>
    /// Confirms user phone number.
    /// </summary>
    [HttpGet("confirm-phone")]
    [AllowAnonymous]
    public async Task<ActionResult<string>> ConfirmPhoneNumberAsync(
        [FromQuery] Guid userId,
        [FromQuery] string code,
        CancellationToken cancellationToken)
    {
        var command = new ConfirmPhoneNumberCommand(userId, code);
        var result = await _sender.Send(command, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.Error.ToBadRequest();
    }

    #endregion

    #region Password Recovery

    /// <summary>
    /// Initiates forgot password flow.
    /// </summary>
    [HttpPost("forgot-password")]
    [AllowAnonymous]
    public async Task<ActionResult<string>> ForgotPasswordAsync(
        ForgotPasswordCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.Error.ToBadRequest();
    }

    /// <summary>
    /// Resets user password with token.
    /// </summary>
    [HttpPost("reset-password")]
    [AllowAnonymous]
    public async Task<ActionResult> ResetPasswordAsync(
        ResetPasswordCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        return result.IsSuccess ? NoContent() : result.Error.ToBadRequest();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Gets client IP address from request headers.
    /// </summary>
    private string? _GetIpAddress() =>
        Request.Headers.ContainsKey("X-Forwarded-For")
            ? Request.Headers["X-Forwarded-For"]
            : HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString() ?? "N/A";

    #endregion
}
