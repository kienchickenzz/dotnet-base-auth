/**
 * RefreshTokenCommandHandler processes token refresh requests.
 *
 * <p>Validates expired token and refresh token, then generates new tokens.</p>
 */
namespace AuthService.Application.Features.Identities.Authentication.Commands.RefreshToken;

using System.Security.Claims;

using AuthService.Application.Common.Messaging;
using AuthService.Application.Common.Extensions.Identity;
using AuthService.Application.Features.Identities.Authentication.Models.Responses;
using AuthService.Application.Features.Identities.Authentication.Services;
using AuthService.Domain.Common;


/// <summary>
/// Handler for RefreshTokenCommand - validates and refreshes JWT tokens.
/// </summary>
public sealed class RefreshTokenCommandHandler : ICommandHandler<RefreshTokenCommand, TokenResponse>
{
    private readonly IAuthenticationService _authService;
    private readonly IJwtTokenGenerator _tokenGenerator;

    public RefreshTokenCommandHandler(
        IAuthenticationService authService,
        IJwtTokenGenerator tokenGenerator)
    {
        _authService = authService;
        _tokenGenerator = tokenGenerator;
    }

    public async Task<Result<TokenResponse>> Handle(
        RefreshTokenCommand request,
        CancellationToken cancellationToken)
    {
        // Extract principal from expired token
        var principal = _tokenGenerator.GetPrincipalFromExpiredToken(request.Token);
        if (principal is null)
        {
            return Result.Failure<TokenResponse>(AuthenticationErrors.InvalidToken);
        }

        // Get user email from claims
        var email = principal.GetEmail();
        if (string.IsNullOrEmpty(email))
        {
            return Result.Failure<TokenResponse>(AuthenticationErrors.InvalidToken);
        }

        // Get user by email
        var userResult = await _authService.GetUserByEmailAsync(email, cancellationToken);
        if (userResult.IsFailure)
        {
            return Result.Failure<TokenResponse>(userResult.Error);
        }

        var user = userResult.Value;

        // Validate refresh token
        var validateResult = await _authService.ValidateRefreshTokenAsync(
            user.Id,
            request.RefreshToken,
            cancellationToken);

        if (validateResult.IsFailure)
        {
            return Result.Failure<TokenResponse>(validateResult.Error);
        }

        // Generate new tokens with roles and permissions
        var accessToken = _tokenGenerator.GenerateAccessToken(
            user.Id,
            user.Email,
            user.FirstName,
            user.LastName,
            user.PhoneNumber,
            user.ImageUrl,
            request.IpAddress,
            user.Roles,
            user.Permissions);

        var refreshToken = _tokenGenerator.GenerateRefreshToken();
        var refreshTokenExpiry = _tokenGenerator.GetRefreshTokenExpiryTime();

        // Update user's refresh token
        await _authService.UpdateRefreshTokenAsync(
            user.Id,
            refreshToken,
            refreshTokenExpiry,
            cancellationToken);

        return new TokenResponse(accessToken, refreshToken, refreshTokenExpiry, user.Roles);
    }
}
