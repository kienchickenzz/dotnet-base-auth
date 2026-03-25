/**
 * LoginCommandHandler processes user authentication requests.
 *
 * <p>Validates credentials, checks user status, and generates JWT tokens.</p>
 */
namespace AuthService.Application.Features.Identities.Authentication.Commands.Login;

using AuthService.Application.Common.Messaging;
using AuthService.Application.Features.Identities.Authentication.Models.Responses;
using AuthService.Application.Features.Identities.Authentication.Services;
using AuthService.Domain.Common;


/// <summary>
/// Handler for LoginCommand - authenticates user and returns tokens.
/// </summary>
public sealed class LoginCommandHandler : ICommandHandler<LoginCommand, TokenResponse>
{
    private readonly IAuthenticationService _authService;
    private readonly IJwtTokenGenerator _tokenGenerator;

    public LoginCommandHandler(
        IAuthenticationService authService,
        IJwtTokenGenerator tokenGenerator)
    {
        _authService = authService;
        _tokenGenerator = tokenGenerator;
    }

    public async Task<Result<TokenResponse>> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        // Validate credentials and get user info
        var userResult = await _authService.ValidateCredentialsAsync(
            request.Email,
            request.Password,
            cancellationToken);

        if (userResult.IsFailure)
        {
            return Result.Failure<TokenResponse>(userResult.Error);
        }

        var user = userResult.Value;

        // Generate tokens
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

        // Update user's refresh token
        await _authService.UpdateRefreshTokenAsync(
            user.Id,
            refreshToken,
            refreshTokenExpiry,
            cancellationToken);

        return new TokenResponse(accessToken, refreshToken, refreshTokenExpiry);
    }
}
