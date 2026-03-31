/**
 * AuthenticationService implements user authentication operations.
 *
 * <p>Uses ASP.NET Core Identity for credential validation and user management.</p>
 */
namespace AuthService.Identity.Services;

using Microsoft.AspNetCore.Identity;

using AuthService.Application.Features.Identities.Authentication;
using AuthService.Application.Features.Identities.Authentication.Services;
using AuthService.Domain.Common;
using AuthService.Identity.Entities;


/// <summary>
/// Authentication service using ASP.NET Core Identity.
/// </summary>
internal sealed class AuthenticationService : IAuthenticationService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public AuthenticationService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    /// <inheritdoc />
    public async Task<Result<AuthenticatedUserInfo>> ValidateCredentialsAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default)
    {
        // Find user by email
        var user = await _userManager.FindByEmailAsync(email.Trim().Normalize());
        if (user is null)
        {
            return Result.Failure<AuthenticatedUserInfo>(AuthenticationErrors.InvalidCredentials);
        }

        // Validate password
        if (!await _userManager.CheckPasswordAsync(user, password))
        {
            return Result.Failure<AuthenticatedUserInfo>(AuthenticationErrors.InvalidCredentials);
        }

        // Check user status
        if (!user.IsActive)
        {
            return Result.Failure<AuthenticatedUserInfo>(AuthenticationErrors.UserNotActive);
        }

        // Check email confirmation
        if (!user.EmailConfirmed)
        {
            return Result.Failure<AuthenticatedUserInfo>(AuthenticationErrors.EmailNotConfirmed);
        }

        return _MapToUserInfo(user);
    }

    /// <inheritdoc />
    public async Task<Result<AuthenticatedUserInfo>> GetUserByEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
        {
            return Result.Failure<AuthenticatedUserInfo>(AuthenticationErrors.InvalidCredentials);
        }

        return _MapToUserInfo(user);
    }

    /// <inheritdoc />
    public async Task<Result> ValidateRefreshTokenAsync(
        Guid userId,
        string refreshToken,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null)
        {
            return Result.Failure(AuthenticationErrors.InvalidCredentials);
        }

        if (user.RefreshToken != refreshToken)
        {
            return Result.Failure(AuthenticationErrors.InvalidRefreshToken);
        }

        if (user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            return Result.Failure(AuthenticationErrors.InvalidRefreshToken);
        }

        return Result.Success();
    }

    /// <inheritdoc />
    public async Task UpdateRefreshTokenAsync(
        Guid userId,
        string refreshToken,
        DateTime expiryTime,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is not null)
        {
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = expiryTime;
            await _userManager.UpdateAsync(user);
        }
    }

    /// <inheritdoc />
    public async Task RevokeRefreshTokenAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is not null)
        {
            user.RefreshToken = string.Empty;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(-1);
            await _userManager.UpdateAsync(user);
        }
    }

    /// <summary>
    /// Maps ApplicationUser to AuthenticatedUserInfo.
    /// </summary>
    private static AuthenticatedUserInfo _MapToUserInfo(ApplicationUser user)
    {
        return new AuthenticatedUserInfo(
            user.Id,
            user.Email!,
            user.FirstName,
            user.LastName,
            user.PhoneNumber,
            user.ImageUrl);
    }
}
