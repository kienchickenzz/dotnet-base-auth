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
        // ┌─────────────────────────────────────────────────────────────────┐
        // │ STEP 1: Get info FROM OAUTH PROVIDER (Google, Facebook, etc.)  │
        // │ Data source: Temporary cookie set by OAuth middleware          │
        // │ Returns: ProviderKey (Google's user ID), email, name           │
        // └─────────────────────────────────────────────────────────────────┘
        var externalInfoResult = await _externalAuthService.GetExternalLoginInfoAsync();
        if (externalInfoResult.IsFailure)
            return Result.Failure<ExternalLoginCallbackResponse>(externalInfoResult.Error);

        var externalInfo = externalInfoResult.Value;

        // ┌─────────────────────────────────────────────────────────────────┐
        // │ STEP 2: Find user FROM LOCAL DATABASE using ProviderKey        │
        // │ Data source: AspNetUserLogins + AspNetUsers tables             │
        // │ Returns: Our stored profile (may differ from Google's data)    │
        // └─────────────────────────────────────────────────────────────────┘
        var loginResult = await _externalAuthService.ExternalLoginAsync(
            externalInfo.LoginProvider,
            externalInfo.ProviderKey,
            cancellationToken);

        // ┌─────────────────────────────────────────────────────────────────┐
        // │ CASE A: User EXISTS in our database → Generate JWT             │
        // │ Use profile FROM DATABASE (not from Google) for JWT claims     │
        // └─────────────────────────────────────────────────────────────────┘
        if (loginResult.IsSuccess)
        {
            // user = profile from OUR database, not from Google
            var user = loginResult.Value;

            // Generate JWT using OUR stored data (user.Email may differ from externalInfo.Email)
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

        // ┌─────────────────────────────────────────────────────────────────┐
        // │ CASE B: User NOT in database → Return Google's info for form   │
        // │ Controller will show confirmation form pre-filled with         │
        // │ externalInfo (email, name from Google) for user to review      │
        // └─────────────────────────────────────────────────────────────────┘
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
