/**
 * LoginViewModel represents the data for customer login form.
 *
 * <p>Contains user credentials and authentication options.</p>
 */

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication;

namespace AuthService.Web.Areas.Customer.Features.Auth.Login.Models;

/// <summary>
/// ViewModel for customer login page.
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

    /// <summary>
    /// List of external authentication providers.
    /// </summary>
    public IList<AuthenticationScheme> ExternalLogins { get; set; } = [];
}
