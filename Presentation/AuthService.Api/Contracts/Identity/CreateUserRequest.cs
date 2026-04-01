/**
 * CreateUserRequest is the API request for creating a new user.
 *
 * <p>Used by UsersController.CreateAsync endpoint.</p>
 */
namespace AuthService.Api.Contracts.Identity;


/// <summary>
/// Request to create a new user.
/// </summary>
/// <param name="FirstName">User's first name.</param>
/// <param name="LastName">User's last name.</param>
/// <param name="Email">User's email address.</param>
/// <param name="UserName">User's username.</param>
/// <param name="Password">User's password.</param>
/// <param name="ConfirmPassword">Password confirmation.</param>
/// <param name="PhoneNumber">User's phone number (optional).</param>
public sealed record CreateUserRequest(
    string FirstName,
    string LastName,
    string Email,
    string UserName,
    string Password,
    string ConfirmPassword,
    string? PhoneNumber);
