/**
 * ExternalLoginCallbackCommandHandler processes OAuth callback.
 *
 * <p>Checks if user exists, generates JWT or returns registration info.</p>
 */
namespace AuthService.Application.Features.Identities.Authentication.Commands.ExternalLogin;

using Microsoft.Extensions.Logging;

using AuthService.Application.Common.Abstractions.Identity;
using AuthService.Application.Common.Messaging;
using AuthService.Application.Features.Identities.Authentication.Services;
using AuthService.Domain.Common;


/// <summary>
/// Handler for ExternalLoginCallbackCommand.
/// </summary>
public sealed class ExternalLoginCallbackCommandHandler
    : ICommandHandler<ExternalLoginCallbackCommand, ExternalLoginCallbackResponse>
{
    private readonly IExternalAuthService _externalAuthService;
    private readonly IJwtTokenGenerator _tokenGenerator;
    private readonly IAuthenticationService _authService;
    private readonly ILogger<ExternalLoginCallbackCommandHandler> _logger;

    public ExternalLoginCallbackCommandHandler(
        IExternalAuthService externalAuthService,
        IJwtTokenGenerator tokenGenerator,
        IAuthenticationService authService,
        ILogger<ExternalLoginCallbackCommandHandler> logger)
    {
        _externalAuthService = externalAuthService;
        _tokenGenerator = tokenGenerator;
        _authService = authService;
        _logger = logger;
    }

    public async Task<Result<ExternalLoginCallbackResponse>> Handle(
        ExternalLoginCallbackCommand request,
        CancellationToken cancellationToken)
    {
        // Get external login info from OAuth callback
        var externalInfoResult = await _externalAuthService.GetExternalLoginInfoAsync();
        if (externalInfoResult.IsFailure)
            return Result.Failure<ExternalLoginCallbackResponse>(externalInfoResult.Error);

        var externalInfo = externalInfoResult.Value;

        // Try to sign in with existing external login
        var loginResult = await _externalAuthService.ExternalLoginAsync(
            externalInfo.LoginProvider,
            externalInfo.ProviderKey,
            cancellationToken);

        // User exists - generate JWT
        if (loginResult.IsSuccess)
        {
            var user = loginResult.Value;

            var accessToken = _tokenGenerator.GenerateAccessToken(
                user.Id,
                user.Email,
                user.FirstName,
                user.LastName,
                user.PhoneNumber,
                user.ImageUrl,
                request.IpAddress);

            var refreshToken = _tokenGenerator.GenerateRefreshToken();
            var refreshTokenExpiry = _tokenGenerator.GetRefreshTokenExpiryTime();

            // Update refresh token
            await _authService.UpdateRefreshTokenAsync(
                user.Id,
                refreshToken,
                refreshTokenExpiry,
                cancellationToken);

            _logger.LogInformation(
                "User {Email} logged in with {Provider}.",
                user.Email,
                externalInfo.LoginProvider);

            return new ExternalLoginCallbackResponse
            {
                IsExistingUser = true,
                Token = new Models.Responses.TokenResponse(
                    accessToken,
                    refreshToken,
                    refreshTokenExpiry)
            };
        }

        // User doesn't exist - return info for registration
        _logger.LogInformation(
            "New user from {Provider}, email: {Email}",
            externalInfo.LoginProvider,
            externalInfo.Email);

        return new ExternalLoginCallbackResponse
        {
            IsExistingUser = false,
            ExternalLoginInfo = externalInfo
        };
    }
}
