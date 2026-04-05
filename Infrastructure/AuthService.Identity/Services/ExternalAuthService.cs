/**
 * ExternalAuthService implements external authentication operations.
 *
 * <p>Uses SignInManager for OAuth provider integration.</p>
 */
namespace AuthService.Identity.Services;

using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

using AuthService.Application.Common.Abstractions.Identity;
using AuthService.Application.Common.Abstractions.Identity.Models;
using AuthService.Application.Features.Identities.Authentication;
using AuthService.Application.Features.Identities.Roles;
using AuthService.Domain.Common;
using AuthService.Identity.DatabaseContext;
using AuthService.Identity.Entities;


/// <summary>
/// Implementation of external authentication service using ASP.NET Identity.
/// </summary>
public class ExternalAuthService : IExternalAuthService
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationIdentityDbContext _db;

    public ExternalAuthService(
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager,
        ApplicationIdentityDbContext db)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _db = db;
    }

    /// <inheritdoc />
    public async Task<IList<AuthenticationScheme>> GetExternalAuthSchemesAsync() =>
        (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

    /// <inheritdoc />
    public AuthenticationProperties ConfigureExternalAuthProperties(string provider, string redirectUrl) =>
        _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);

    /// <inheritdoc />
    public async Task<Result<ExternalLoginInfoDto>> GetExternalLoginInfoAsync()
    {
        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (info == null)
            return Result.Failure<ExternalLoginInfoDto>(AuthenticationErrors.ExternalLoginInfoNotFound);

        // Extract user info from claims
        var email = info.Principal.FindFirstValue(ClaimTypes.Email);
        var firstName = info.Principal.FindFirstValue(ClaimTypes.GivenName)
                       ?? info.Principal.FindFirstValue(ClaimTypes.Name)?.Split(' ').FirstOrDefault();
        var lastName = info.Principal.FindFirstValue(ClaimTypes.Surname)
                      ?? info.Principal.FindFirstValue(ClaimTypes.Name)?.Split(' ').LastOrDefault();

        return new ExternalLoginInfoDto(
            info.LoginProvider,
            info.ProviderKey,
            info.ProviderDisplayName ?? info.LoginProvider,
            email,
            firstName,
            lastName);
    }

    /// <inheritdoc />
    public async Task<Result<UserDto>> ExternalLoginAsync(
        string loginProvider,
        string providerKey,
        CancellationToken cancellationToken = default)
    {
        // Try external login sign-in (bypasses two-factor)
        var result = await _signInManager.ExternalLoginSignInAsync(
            loginProvider,
            providerKey,
            isPersistent: false,
            bypassTwoFactor: true);

        if (!result.Succeeded)
            return Result.Failure<UserDto>(AuthenticationErrors.ExternalLoginFailed);

        // Get user by external login
        var user = await _userManager.FindByLoginAsync(loginProvider, providerKey);
        if (user == null)
            return Result.Failure<UserDto>(AuthenticationErrors.UserNotFound);

        return _MapToUserDto(user);
    }

    /// <inheritdoc />
    public async Task<Result<Guid>> CreateUserWithExternalLoginAsync(
        CreateUserDto userDto,
        ExternalLoginInfoDto externalLogin,
        string origin,
        CancellationToken cancellationToken = default)
    {
        var user = new ApplicationUser
        {
            UserName = userDto.Email,
            Email = userDto.Email,
            FirstName = userDto.FirstName,
            LastName = userDto.LastName,
            PhoneNumber = userDto.PhoneNumber,
            IsActive = true,
            EmailConfirmed = true // External providers verify email
        };

        // Create user without password
        var createResult = await _userManager.CreateAsync(user);
        if (!createResult.Succeeded)
        {
            var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
            return Result.Failure<Guid>(new Error("User.CreateFailed", errors));
        }

        // Save user first (required because UserStore has AutoSaveChanges = false)
        await _db.SaveChangesAsync(cancellationToken);

        // Link external login
        var loginInfo = new UserLoginInfo(
            externalLogin.LoginProvider,
            externalLogin.ProviderKey,
            externalLogin.ProviderDisplayName);

        var addLoginResult = await _userManager.AddLoginAsync(user, loginInfo);
        if (!addLoginResult.Succeeded)
        {
            // Rollback: delete user if linking fails
            await _userManager.DeleteAsync(user);
            await _db.SaveChangesAsync(cancellationToken);
            return Result.Failure<Guid>(AuthenticationErrors.ExternalLoginLinkFailed);
        }

        // Assign default Customer role
        await _userManager.AddToRoleAsync(user, Roles.Customer);

        // Note: External login and role are saved by TransactionPipelineBehavior
        // to maintain same pattern as IdentityUserService.CreateAsync

        return user.Id;
    }

    /// <summary>
    /// Maps ApplicationUser to UserDto.
    /// </summary>
    private static UserDto _MapToUserDto(ApplicationUser user) =>
        new UserDto
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
