/**
 * ExternalLoginConfirmationCommandHandler creates user and links external login.
 *
 * <p>Generates JWT after successful user creation.</p>
 */
namespace AuthService.Application.Features.Identities.Authentication.Commands.ExternalLogin;

using Microsoft.Extensions.Logging;

using AuthService.Application.Common.Abstractions.Identity;
using AuthService.Application.Common.Abstractions.Identity.Models;
using AuthService.Application.Common.Messaging;
using AuthService.Application.Features.Identities.Authentication.Models.Responses;
using AuthService.Application.Features.Identities.Authentication.Services;
using AuthService.Domain.Common;


/// <summary>
/// Handler for ExternalLoginConfirmationCommand.
/// </summary>
public sealed class ExternalLoginConfirmationCommandHandler
    : ICommandHandler<ExternalLoginConfirmationCommand, TokenResponse>
{
    private readonly IExternalAuthService _externalAuthService;
    private readonly IJwtTokenGenerator _tokenGenerator;
    private readonly IAuthenticationService _authService;
    private readonly IIdentityUserService _userService;
    private readonly ILogger<ExternalLoginConfirmationCommandHandler> _logger;

    public ExternalLoginConfirmationCommandHandler(
        IExternalAuthService externalAuthService,
        IJwtTokenGenerator tokenGenerator,
        IAuthenticationService authService,
        IIdentityUserService userService,
        ILogger<ExternalLoginConfirmationCommandHandler> logger)
    {
        _externalAuthService = externalAuthService;
        _tokenGenerator = tokenGenerator;
        _authService = authService;
        _userService = userService;
        _logger = logger;
    }

    public async Task<Result<TokenResponse>> Handle(
        ExternalLoginConfirmationCommand request,
        CancellationToken cancellationToken)
    {
        // Get external login info (still available in HttpContext)
        var externalInfoResult = await _externalAuthService.GetExternalLoginInfoAsync();
        if (externalInfoResult.IsFailure)
            return Result.Failure<TokenResponse>(externalInfoResult.Error);

        var externalInfo = externalInfoResult.Value;

        // Create user DTO with password for local login capability
        var createUserDto = new CreateUserDto(
            request.FirstName,
            request.LastName,
            request.Email,
            request.Email,  // UserName = Email
            request.Password,
            request.PhoneNumber);

        // Create user and link external login
        var createResult = await _externalAuthService.CreateUserWithExternalLoginAsync(
            createUserDto,
            externalInfo,
            request.Origin,
            cancellationToken);

        if (createResult.IsFailure)
            return Result.Failure<TokenResponse>(createResult.Error);

        var userId = createResult.Value;

        // Get created user for token generation
        var userResult = await _userService.GetByIdAsync(userId, cancellationToken);
        if (userResult.IsFailure)
            return Result.Failure<TokenResponse>(userResult.Error);

        var user = userResult.Value;

        // Generate JWT
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
            "User {Email} created via {Provider}.",
            user.Email,
            externalInfo.LoginProvider);

        return new TokenResponse(accessToken, refreshToken, refreshTokenExpiry);
    }
}
