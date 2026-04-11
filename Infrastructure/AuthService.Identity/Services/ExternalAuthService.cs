/**
 * ExternalAuthService implements external authentication operations.
 *
 * <p>Uses SignInManager for OAuth provider integration.</p>
 *
 * <p>IMPORTANT: This service works with TWO data sources:</p>
 * <ul>
 *   <li>OAuth Provider (Google, Facebook): GetExternalLoginInfoAsync</li>
 *   <li>Local Database: ExternalLoginAsync, CreateUserWithExternalLoginAsync</li>
 * </ul>
 */
namespace AuthService.Identity.Services;

using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using AuthService.Application.Common.Abstractions.Identity;
using AuthService.Application.Common.Abstractions.Identity.Models;
using AuthService.Application.Features.Identities.Authentication;
using AuthService.Application.Features.Identities.Roles;
using AuthService.Domain.Common;
using AuthService.Domain.Constants.Identity;
using AuthService.Identity.DatabaseContext;
using AuthService.Identity.Entities;


/// <summary>
/// Implementation of external authentication service using ASP.NET Identity.
/// </summary>
public class ExternalAuthService : IExternalAuthService
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly ApplicationIdentityDbContext _db;

    public ExternalAuthService(
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        ApplicationIdentityDbContext db)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _roleManager = roleManager;
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
        // Read temporary "Identity.External" cookie set by OAuth middleware
        // This cookie was created at /signin-google after user consented
        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (info == null)
            return Result.Failure<ExternalLoginInfoDto>(AuthenticationErrors.ExternalLoginInfoNotFound);

        // Extract user info from OAuth claims (data from Google, not our DB)
        // ProviderKey = Google's unique user ID (e.g., "117234567890123456")
        // This is NOT the email - it's a stable identifier for the Google account
        var email = info.Principal.FindFirstValue(ClaimTypes.Email);
        var firstName = info.Principal.FindFirstValue(ClaimTypes.GivenName)
                       ?? info.Principal.FindFirstValue(ClaimTypes.Name)?.Split(' ').FirstOrDefault();
        var lastName = info.Principal.FindFirstValue(ClaimTypes.Surname)
                      ?? info.Principal.FindFirstValue(ClaimTypes.Name)?.Split(' ').LastOrDefault();

        return new ExternalLoginInfoDto(
            info.LoginProvider,    // "Google"
            info.ProviderKey,      // Google's unique user ID (NOT email)
            info.ProviderDisplayName ?? info.LoginProvider,
            email,                 // Email from Google profile
            firstName,             // Name from Google profile
            lastName);
    }

    /// <inheritdoc />
    public async Task<Result<UserDto>> ExternalLoginAsync(
        string loginProvider,
        string providerKey,
        CancellationToken cancellationToken = default)
    {
        // Query AspNetUserLogins table: (LoginProvider, ProviderKey) → UserId
        // Then join with AspNetUsers to get full profile
        // Note: Don't use SignInAsync - we use JWT authentication, not Identity cookies
        var user = await _userManager.FindByLoginAsync(loginProvider, providerKey);
        if (user == null)
            return Result.Failure<UserDto>(AuthenticationErrors.ExternalLoginFailed);

        // Check user status (from our DB, not from Google)
        if (!user.IsActive)
            return Result.Failure<UserDto>(AuthenticationErrors.UserNotActive);

        // Return OUR stored profile with roles and permissions
        return await _MapToUserDtoAsync(user, cancellationToken);
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

        // Create user with password for local login capability
        var createResult = await _userManager.CreateAsync(user, userDto.Password);
        if (!createResult.Succeeded)
        {
            var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
            return Result.Failure<Guid>(new Error("User.CreateFailed", errors));
        }

        // Save user first (required for AddLoginAsync to reference valid user)
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
    /// Maps ApplicationUser to UserDto with roles and permissions.
    /// </summary>
    private async Task<UserDto> _MapToUserDtoAsync(
        ApplicationUser user,
        CancellationToken cancellationToken)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var permissions = await _GetPermissionsAsync(roles, cancellationToken);

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
            PhoneNumberConfirmed = user.PhoneNumberConfirmed,
            Roles = roles.ToList(),
            Permissions = permissions
        };
    }

    /// <summary>
    /// Gets permissions from role claims for the specified roles.
    /// </summary>
    private async Task<List<string>> _GetPermissionsAsync(
        IList<string> roleNames,
        CancellationToken cancellationToken)
    {
        var permissions = new List<string>();

        var roles = await _roleManager.Roles
            .Where(r => roleNames.Contains(r.Name!))
            .ToListAsync(cancellationToken);

        foreach (var role in roles)
        {
            var roleClaims = await _db.RoleClaims
                .Where(rc => rc.RoleId == role.Id && rc.ClaimType == Claims.Permission)
                .Select(rc => rc.ClaimValue!)
                .ToListAsync(cancellationToken);

            permissions.AddRange(roleClaims);
        }

        return permissions.Distinct().ToList();
    }
}
