/**
 * CreateUserDto contains data for creating a new user.
 *
 * <p>Used by IIdentityUserService.CreateAsync.</p>
 */
namespace AuthService.Application.Common.Abstractions.Identity.Models;


/// <summary>
/// Data for creating a new user.
/// </summary>
public sealed record CreateUserDto(
    string FirstName,
    string LastName,
    string Email,
    string UserName,
    string Password,
    string? PhoneNumber);
