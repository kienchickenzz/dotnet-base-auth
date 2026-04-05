/**
 * IdentityAuthService implements IIdentityAuthService using ASP.NET Core Identity.
 *
 * <p>Provides authentication operations.</p>
 */
namespace AuthService.Identity.Services;

using Microsoft.AspNetCore.Identity;

using AuthService.Application.Common.Abstractions.Identity;
using AuthService.Application.Common.Abstractions.Identity.Models;
using AuthService.Domain.Common;
using AuthService.Identity.DatabaseContext;
using AuthService.Identity.Entities;


/// <summary>
/// Authentication service using ASP.NET Core Identity.
/// </summary>
internal sealed class IdentityAuthService : IIdentityAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationIdentityDbContext _db;

    public IdentityAuthService(
        UserManager<ApplicationUser> userManager,
        ApplicationIdentityDbContext db)
    {
        _userManager = userManager;
        _db = db;
    }

    /// <inheritdoc />
    public async Task<Result<UserDto>> ValidateCredentialsAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default)
    {
        // Find user by email
        var user = await _userManager.FindByEmailAsync(email.Trim().Normalize());
        if (user is null)
        {
            return Result.Failure<UserDto>(AuthErrors.InvalidCredentials);
        }

        // Validate password
        if (!await _userManager.CheckPasswordAsync(user, password))
        {
            return Result.Failure<UserDto>(AuthErrors.InvalidCredentials);
        }

        // Check user status
        if (!user.IsActive)
        {
            return Result.Failure<UserDto>(AuthErrors.UserNotActive);
        }

        // Check email confirmation
        if (!user.EmailConfirmed)
        {
            return Result.Failure<UserDto>(AuthErrors.EmailNotConfirmed);
        }

        return _MapToDto(user);
    }

    /// <inheritdoc />
    public async Task<Result<UserDto>> GetUserByEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
        {
            return Result.Failure<UserDto>(AuthErrors.InvalidCredentials);
        }

        return _MapToDto(user);
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
            return Result.Failure(AuthErrors.InvalidCredentials);
        }

        if (user.RefreshToken != refreshToken)
        {
            return Result.Failure(AuthErrors.InvalidRefreshToken);
        }

        if (user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            return Result.Failure(AuthErrors.InvalidRefreshToken);
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
            // Update directly without UserManager.UpdateAsync to avoid
            // ConcurrencyStamp conflict when entity is already tracked
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = expiryTime;
            // Note: Don't call SaveChangesAsync here - let TransactionPipelineBehavior handle it
        }
    }

    /// <summary>
    /// Maps ApplicationUser to UserDto.
    /// </summary>
    private static UserDto _MapToDto(ApplicationUser user)
    {
        return new UserDto
        {
            Id = user.Id,
            UserName = user.UserName!,
            Email = user.Email!,
            FirstName = user.FirstName,
            LastName = user.LastName,
            PhoneNumber = user.PhoneNumber,
            ImageUrl = user.ImageUrl,
            IsActive = user.IsActive,
            EmailConfirmed = user.EmailConfirmed,
            PhoneNumberConfirmed = user.PhoneNumberConfirmed
        };
    }
}


/// <summary>
/// Error definitions for authentication operations.
/// </summary>
internal static class AuthErrors
{
    public static readonly Error InvalidCredentials = new("Auth.InvalidCredentials", "Invalid email or password.");
    public static readonly Error UserNotActive = new("Auth.UserNotActive", "User account is not active. Please contact the administrator.");
    public static readonly Error EmailNotConfirmed = new("Auth.EmailNotConfirmed", "Email address has not been confirmed.");
    public static readonly Error InvalidRefreshToken = new("Auth.InvalidRefreshToken", "Invalid or expired refresh token.");
}
