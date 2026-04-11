/**
 * LoginViewModel represents the data for login form.
 *
 * <p>Contains user credentials for JWT-based authentication.</p>
 */

using System.ComponentModel.DataAnnotations;

namespace AuthService.Web.Models.Auth;

/// <summary>
/// ViewModel for login page.
/// </summary>
public class LoginViewModel
{
    /// <summary>
    /// User's email address.
    /// </summary>
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// User's password.
    /// </summary>
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Whether to persist the login session.
    /// </summary>
    [Display(Name = "Remember me?")]
    public bool RememberMe { get; set; }

    /// <summary>
    /// URL to redirect after successful login.
    /// </summary>
    public string? ReturnUrl { get; set; }
}
