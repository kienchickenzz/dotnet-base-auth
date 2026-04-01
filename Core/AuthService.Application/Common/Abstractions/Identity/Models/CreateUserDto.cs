/**
 * CreateUserDto contains data for creating a new user.
 *
 * <p>Used by IIdentityUserService.CreateAsync.</p>
 */
namespace AuthService.Application.Common.Abstractions.Identity.Models;


/// <summary>
/// Data for creating a new user.
/// </summary>
/// <param name="FirstName">User's first name.</param>
/// <param name="LastName">User's last name.</param>
/// <param name="Email">User's email address.</param>
/// <param name="UserName">User's login username.</param>
/// <param name="Password">User's password.</param>
/// <param name="PhoneNumber">User's phone number (optional).</param>
public sealed record CreateUserDto(
    string FirstName,
    string LastName,
    string Email,
    string UserName,
    string Password,
    string? PhoneNumber);
