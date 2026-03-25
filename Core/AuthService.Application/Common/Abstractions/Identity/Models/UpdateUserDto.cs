/**
 * UpdateUserDto contains data for updating user profile.
 *
 * <p>Used by IIdentityUserService.UpdateAsync.</p>
 */
namespace AuthService.Application.Common.Abstractions.Identity.Models;


/// <summary>
/// Data for updating user profile.
/// </summary>
public sealed record UpdateUserDto(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string? PhoneNumber);
