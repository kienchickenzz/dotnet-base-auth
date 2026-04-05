/**
 * IIdentityUserService defines contract for user management operations.
 *
 * <p>Abstracts user operations from identity framework implementation.</p>
 */
namespace AuthService.Application.Common.Abstractions.Identity;

using AuthService.Application.Common.Abstractions.Identity.Models;
using AuthService.Domain.Common;


/// <summary>
/// Service for user management operations.
/// </summary>
public interface IIdentityUserService
{
    // ============ Queries ============

    /// <summary>
    /// Gets all users.
    /// </summary>
    Task<Result<List<UserDto>>> GetUsersAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets user by Id.
    /// </summary>
    Task<Result<UserDto>> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets user by email.
    /// </summary>
    Task<Result<UserDto>> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets user count.
    /// </summary>
    Task<int> GetCountAsync(CancellationToken cancellationToken = default);

    // ============ Existence Checks ============

    /// <summary>
    /// Checks if user exists with given username.
    /// </summary>
    Task<bool> ExistsWithNameAsync(string name);

    /// <summary>
    /// Checks if user exists with given email.
    /// </summary>
    Task<bool> ExistsWithEmailAsync(string email, Guid? exceptId = null);

    /// <summary>
    /// Checks if user exists with given phone number.
    /// </summary>
    Task<bool> ExistsWithPhoneNumberAsync(string phoneNumber, Guid? exceptId = null);

    // ============ Commands ============

    /// <summary>
    /// Creates a new user and sends confirmation email.
    /// </summary>
    /// <param name="user">User data.</param>
    /// <param name="origin">Origin URL for building confirmation link.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result with user Id or error.</returns>
    Task<Result<Guid>> CreateAsync(CreateUserDto user, string origin, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates user profile.
    /// </summary>
    Task<Result> UpdateAsync(UpdateUserDto user, CancellationToken cancellationToken = default);

    /// <summary>
    /// Toggles user active status.
    /// </summary>
    Task<Result> ToggleStatusAsync(Guid userId, bool activate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Soft-deletes a user account.
    /// </summary>
    Task<Result> DeleteAsync(Guid userId, CancellationToken cancellationToken = default);

    // ============ Password ============

    /// <summary>
    /// Changes user password.
    /// </summary>
    Task<Result> ChangePasswordAsync(
        Guid userId,
        string currentPassword,
        string newPassword,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Initiates forgot password flow.
    /// </summary>
    /// <returns>Result with password reset URL or message.</returns>
    Task<Result<string>> ForgotPasswordAsync(
        string email,
        string origin,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Resets user password with token.
    /// </summary>
    Task<Result> ResetPasswordAsync(
        string email,
        string token,
        string newPassword,
        CancellationToken cancellationToken = default);

    // ============ Confirmation ============

    /// <summary>
    /// Confirms user email.
    /// </summary>
    Task<Result<string>> ConfirmEmailAsync(
        Guid userId,
        string code,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Confirms user phone number.
    /// </summary>
    Task<Result<string>> ConfirmPhoneNumberAsync(
        Guid userId,
        string code,
        CancellationToken cancellationToken = default);

    // ============ Roles ============

    /// <summary>
    /// Gets roles for a user.
    /// </summary>
    Task<Result<List<UserRoleDto>>> GetRolesAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Assigns roles to a user.
    /// </summary>
    Task<Result> AssignRolesAsync(
        Guid userId,
        List<UserRoleDto> roles,
        CancellationToken cancellationToken = default);

    // ============ External Logins ============

    /// <summary>
    /// Gets external login providers linked to user's account.
    /// </summary>
    /// <remarks>
    /// Returns list of OAuth providers (Google, Facebook, etc.) that user has linked.
    /// </remarks>
    Task<Result<List<UserExternalLoginDto>>> GetExternalLoginsAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
}
