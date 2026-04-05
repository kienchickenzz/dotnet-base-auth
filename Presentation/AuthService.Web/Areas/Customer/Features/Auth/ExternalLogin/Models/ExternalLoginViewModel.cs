/**
 * ExternalLoginViewModel represents data for external login confirmation.
 *
 * <p>Used when new user needs to provide additional info after OAuth.</p>
 */
namespace AuthService.Web.Areas.Customer.Features.Auth.ExternalLogin.Models;

using System.ComponentModel.DataAnnotations;


/// <summary>
/// ViewModel for external login confirmation page.
/// </summary>
public class ExternalLoginViewModel
{
    /// <summary>
    /// User's email address.
    /// </summary>
    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

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
    /// User's phone number (optional).
    /// </summary>
    [Phone]
    [Display(Name = "Phone Number")]
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Display name of the external provider.
    /// </summary>
    public string ProviderDisplayName { get; set; } = string.Empty;

    /// <summary>
    /// URL to redirect after successful registration.
    /// </summary>
    public string? ReturnUrl { get; set; }
}
