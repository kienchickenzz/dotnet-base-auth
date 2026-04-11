/**
 * RegisterViewModel represents the data for registration form.
 *
 * <p>Contains user info for creating new account.</p>
 */
namespace AuthService.Web.Models.Auth;

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication;


/// <summary>
/// ViewModel for registration page.
/// </summary>
public class RegisterViewModel
{
    /// <summary>
    /// User's first name.
    /// </summary>
    [Required]
    [Display(Name = "First Name")]
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// User's last name.
    /// </summary>
    [Required]
    [Display(Name = "Last Name")]
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// User's email address.
    /// </summary>
    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// User's password.
    /// </summary>
    [Required]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Password confirmation.
    /// </summary>
    [DataType(DataType.Password)]
    [Display(Name = "Confirm Password")]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; } = string.Empty;

    /// <summary>
    /// User's phone number (optional).
    /// </summary>
    [Phone]
    [Display(Name = "Phone Number")]
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// URL to redirect after successful registration.
    /// </summary>
    public string? ReturnUrl { get; set; }

    /// <summary>
    /// Available external login providers.
    /// </summary>
    public IList<AuthenticationScheme> ExternalLogins { get; set; } = new List<AuthenticationScheme>();
}
