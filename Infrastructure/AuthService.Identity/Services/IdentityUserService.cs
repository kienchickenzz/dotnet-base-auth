/**
 * IdentityUserService implements IIdentityUserService using ASP.NET Core Identity.
 *
 * <p>Provides user management operations.</p>
 */
namespace AuthService.Identity.Services;

using System.Text;

using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;

using AuthService.Application.Common.Abstractions.Identity;
using AuthService.Application.Common.Abstractions.Identity.Models;
using AuthService.Application.Features.Identities.Roles;
using AuthService.Domain.Common;
using AuthService.Domain.Events.Identity;
using AuthService.Identity.DatabaseContext;
using AuthService.Identity.Entities;
using AuthService.Identity.Extensions;


/// <summary>
/// User management service using ASP.NET Core Identity.
/// </summary>
internal sealed class IdentityUserService : IIdentityUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ApplicationIdentityDbContext _db;
    private readonly IPublisher _mediator;

    public IdentityUserService(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        SignInManager<ApplicationUser> signInManager,
        ApplicationIdentityDbContext db,
        IPublisher mediator)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _signInManager = signInManager;
        _db = db;
        _mediator = mediator;
    }

    #region Queries

    /// <inheritdoc />
    public async Task<Result<List<UserDto>>> GetUsersAsync(CancellationToken cancellationToken = default)
    {
        var users = await _userManager.Users
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return users.Select(_MapToDto).ToList();
    }

    /// <inheritdoc />
    public async Task<Result<UserDto>> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.Users
            .AsNoTracking()
            .Where(u => u.Id == userId)
            .FirstOrDefaultAsync(cancellationToken);

        if (user is null)
        {
            return Result.Failure<UserDto>(UserErrors.NotFound);
        }

        return _MapToDto(user);
    }

    /// <inheritdoc />
    public async Task<Result<UserDto>> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email.Normalize());

        if (user is null)
        {
            return Result.Failure<UserDto>(UserErrors.NotFound);
        }

        return _MapToDto(user);
    }

    /// <inheritdoc />
    public Task<int> GetCountAsync(CancellationToken cancellationToken = default)
    {
        return _userManager.Users.AsNoTracking().CountAsync(cancellationToken);
    }

    // ============ Existence Checks ============

    /// <inheritdoc />
    public async Task<bool> ExistsWithNameAsync(string name)
    {
        return await _userManager.FindByNameAsync(name) is not null;
    }

    /// <inheritdoc />
    public async Task<bool> ExistsWithEmailAsync(string email, Guid? exceptId = null)
    {
        return await _userManager.FindByEmailAsync(email.Normalize()) is ApplicationUser user
            && user.Id != exceptId;
    }

    /// <inheritdoc />
    public async Task<bool> ExistsWithPhoneNumberAsync(string phoneNumber, Guid? exceptId = null)
    {
        return await _userManager.Users
            .FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber) is ApplicationUser user
            && user.Id != exceptId;
    }

    #endregion

    #region Commands

    /// <inheritdoc />
    public async Task<Result<Guid>> CreateAsync(
        CreateUserDto userDto,
        string origin,
        CancellationToken cancellationToken = default)
    {
        // Transaction is managed by TransactionPipelineBehavior
        var user = new ApplicationUser
        {
            Email = userDto.Email,
            FirstName = userDto.FirstName,
            LastName = userDto.LastName,
            UserName = userDto.UserName,
            PhoneNumber = userDto.PhoneNumber,
            IsActive = true
        };

        var result = await _userManager.CreateAsync(user, userDto.Password);
        if (!result.Succeeded)
        {
            var errors = result.GetErrors();
            return Result.Failure<Guid>(new Error(
                "User.CreateFailed",
                string.Join(", ", errors.Select(e => e.Name))));
        }

        // Save user first (required for AddToRoleAsync which calls UpdateUserAsync internally)
        await _db.SaveChangesAsync(cancellationToken);

        await _userManager.AddToRoleAsync(user, Roles.Customer);

        // Generate email confirmation token (requires user in DB)
        string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        string encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

        // Raise domain event for sending confirmation email (saved to outbox by TransactionPipelineBehavior)
        user.RaiseDomainEvent(new SendEmailConfirmationEvent(
            user.Id,
            user.Email!,
            user.FirstName ?? user.UserName!,
            encodedToken,
            origin));

        await _mediator.Publish(new UserCreatedEvent(user.Id), cancellationToken);

        return user.Id;
    }

    /// <inheritdoc />
    public async Task<Result> UpdateAsync(UpdateUserDto userDto, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userDto.Id.ToString());

        if (user is null)
        {
            return Result.Failure(UserErrors.NotFound);
        }

        user.FirstName = userDto.FirstName;
        user.LastName = userDto.LastName;

        string? phoneNumber = await _userManager.GetPhoneNumberAsync(user);
        if (userDto.PhoneNumber != phoneNumber)
        {
            await _userManager.SetPhoneNumberAsync(user, userDto.PhoneNumber);
        }

        var result = await _userManager.UpdateAsync(user);

        await _signInManager.RefreshSignInAsync(user);

        if (!result.Succeeded)
        {
            var errors = result.GetErrors();
            return Result.Failure(new Error(
                "User.UpdateFailed",
                string.Join(", ", errors.Select(e => e.Name))));
        }

        await _mediator.Publish(new UserUpdatedEvent(user.Id), cancellationToken);

        return Result.Success();
    }

    /// <inheritdoc />
    public async Task<Result> ToggleStatusAsync(Guid userId, bool activate, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.Users
            .Where(u => u.Id == userId)
            .FirstOrDefaultAsync(cancellationToken);

        if (user is null)
        {
            return Result.Failure(UserErrors.NotFound);
        }

        bool isAdmin = await _userManager.IsInRoleAsync(user, Roles.Admin);
        if (isAdmin)
        {
            return Result.Failure(UserErrors.AdminStatusCannotBeToggled);
        }

        user.IsActive = activate;

        await _userManager.UpdateAsync(user);

        await _mediator.Publish(new UserStatusChangedEvent(user.Id, activate), cancellationToken);

        return Result.Success();
    }

    /// <inheritdoc />
    public async Task<Result> DeleteAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.Users
            .Where(u => u.Id == userId)
            .FirstOrDefaultAsync(cancellationToken);

        if (user is null)
        {
            return Result.Failure(UserErrors.NotFound);
        }

        // Prevent deleting admin
        if (await _userManager.IsInRoleAsync(user, Roles.Admin))
        {
            return Result.Failure(UserErrors.AdminCannotBeDeleted);
        }

        // Soft delete: deactivate + clear tokens + set deleted fields
        user.IsActive = false;
        user.DeletedOn = DateTime.UtcNow;
        user.DeletedBy = userId;
        user.RefreshToken = null;
        user.RefreshTokenExpiryTime = DateTime.UtcNow;

        await _userManager.UpdateAsync(user);

        return Result.Success();
    }

    // ============ Password ============

    /// <inheritdoc />
    public async Task<Result> ChangePasswordAsync(
        Guid userId,
        string currentPassword,
        string newPassword,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user is null)
        {
            return Result.Failure(UserErrors.NotFound);
        }

        var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);

        if (!result.Succeeded)
        {
            var errors = result.GetErrors();
            return Result.Failure(new Error(
                "User.ChangePasswordFailed",
                string.Join(", ", errors.Select(e => e.Name))));
        }

        return Result.Success();
    }

    /// <inheritdoc />
    public async Task<Result<string>> ForgotPasswordAsync(
        string email,
        string origin,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email.Normalize());
        if (user is null || !await _userManager.IsEmailConfirmedAsync(user))
        {
            // Don't reveal that the user does not exist or is not confirmed
            return Result.Failure<string>(new Error(
                "User.ForgotPasswordFailed",
                "An Error has occurred!"));
        }

        string code = await _userManager.GeneratePasswordResetTokenAsync(user);
        const string route = "account/reset-password";
        var endpointUri = new Uri(string.Concat($"{origin}/", route));
        string passwordResetUrl = QueryHelpers.AddQueryString(endpointUri.ToString(), "Token", code);

        // TODO: Send email via IMailService

        return "Password Reset Mail has been sent to your authorized Email.";
    }

    /// <inheritdoc />
    public async Task<Result> ResetPasswordAsync(
        string email,
        string token,
        string newPassword,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email.Normalize());

        if (user is null)
        {
            return Result.Failure(UserErrors.ResetPasswordFailed);
        }

        var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

        if (!result.Succeeded)
        {
            return Result.Failure(UserErrors.ResetPasswordFailed);
        }

        return Result.Success();
    }

    // ============ Confirmation ============

    /// <inheritdoc />
    public async Task<Result<string>> ConfirmEmailAsync(
        Guid userId,
        string code,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.Users
            .Where(u => u.Id == userId && !u.EmailConfirmed)
            .FirstOrDefaultAsync(cancellationToken);

        if (user is null)
        {
            return Result.Failure<string>(UserErrors.ConfirmEmailFailed);
        }

        string decodedCode = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
        var result = await _userManager.ConfirmEmailAsync(user, decodedCode);

        if (!result.Succeeded)
        {
            return Result.Failure<string>(new Error(
                "User.ConfirmEmailFailed",
                $"An error occurred while confirming {user.Email}"));
        }

        return $"Account Confirmed for E-Mail {user.Email}. You can now use the /api/tokens endpoint to generate JWT.";
    }

    /// <inheritdoc />
    public async Task<Result<string>> ConfirmPhoneNumberAsync(
        Guid userId,
        string code,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user is null)
        {
            return Result.Failure<string>(UserErrors.ConfirmPhoneFailed);
        }

        if (string.IsNullOrEmpty(user.PhoneNumber))
        {
            return Result.Failure<string>(UserErrors.ConfirmPhoneFailed);
        }

        var result = await _userManager.ChangePhoneNumberAsync(user, user.PhoneNumber, code);

        if (!result.Succeeded)
        {
            return Result.Failure<string>(new Error(
                "User.ConfirmPhoneFailed",
                $"An error occurred while confirming {user.PhoneNumber}"));
        }

        return user.PhoneNumberConfirmed
            ? $"Account Confirmed for Phone Number {user.PhoneNumber}. You can now use the /api/tokens endpoint to generate JWT."
            : $"Account Confirmed for Phone Number {user.PhoneNumber}. You should confirm your E-mail before using the /api/tokens endpoint to generate JWT.";
    }

    // ============ Roles ============

    /// <inheritdoc />
    public async Task<Result<List<UserRoleDto>>> GetRolesAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null)
        {
            return Result.Failure<List<UserRoleDto>>(UserErrors.NotFound);
        }

        var roles = await _roleManager.Roles
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        if (roles is null)
        {
            return Result.Failure<List<UserRoleDto>>(UserErrors.RolesNotFound);
        }

        var userRoles = new List<UserRoleDto>();
        foreach (var role in roles)
        {
            userRoles.Add(new UserRoleDto
            {
                RoleId = role.Id,
                RoleName = role.Name!,
                Description = role.Description,
                IsEnabled = await _userManager.IsInRoleAsync(user, role.Name!)
            });
        }

        return userRoles;
    }

    /// <inheritdoc />
    public async Task<Result> AssignRolesAsync(
        Guid userId,
        List<UserRoleDto> roles,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.Users
            .Where(u => u.Id == userId)
            .FirstOrDefaultAsync(cancellationToken);

        if (user is null)
        {
            return Result.Failure(UserErrors.NotFound);
        }

        foreach (var userRole in roles)
        {
            if (await _roleManager.FindByNameAsync(userRole.RoleName) is not null)
            {
                if (userRole.IsEnabled)
                {
                    if (!await _userManager.IsInRoleAsync(user, userRole.RoleName))
                    {
                        await _userManager.AddToRoleAsync(user, userRole.RoleName);
                    }
                }
                else
                {
                    await _userManager.RemoveFromRoleAsync(user, userRole.RoleName);
                }
            }
        }

        await _mediator.Publish(new UserRolesUpdatedEvent(user.Id), cancellationToken);

        return Result.Success();
    }

    #endregion

    #region Private Helpers

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

    #endregion
}


/// <summary>
/// Error definitions for user operations.
/// </summary>
internal static class UserErrors
{
    public static readonly Error NotFound = new("User.NotFound", "User Not Found.");
    public static readonly Error AdminStatusCannotBeToggled = new("User.AdminStatusCannotBeToggled", "Administrators Profile's Status cannot be toggled.");
    public static readonly Error AdminCannotBeDeleted = new("User.AdminCannotBeDeleted", "Administrator account cannot be deleted.");
    public static readonly Error ResetPasswordFailed = new("User.ResetPasswordFailed", "An Error has occurred!");
    public static readonly Error ConfirmEmailFailed = new("User.ConfirmEmailFailed", "An error occurred while confirming E-Mail.");
    public static readonly Error ConfirmPhoneFailed = new("User.ConfirmPhoneFailed", "An error occurred while confirming Mobile Phone.");
    public static readonly Error RolesNotFound = new("User.RolesNotFound", "Roles Not Found.");
}
