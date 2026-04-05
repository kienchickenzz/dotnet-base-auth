/**
 * ProfileViewModel represents user profile data for display.
 *
 * <p>Read-only view of current user's profile information.</p>
 */
namespace AuthService.Web.Areas.Customer.Features.Profile.Models;


/// <summary>
/// ViewModel for displaying user profile.
/// </summary>
public class ProfileViewModel
{
    /// <summary>
    /// User's email address.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// User's first name.
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// User's last name.
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// User's full name (computed).
    /// </summary>
    public string FullName => $"{FirstName} {LastName}".Trim();

    /// <summary>
    /// User's phone number.
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// URL to user's profile image.
    /// </summary>
    public string? ImageUrl { get; set; }

    /// <summary>
    /// Whether email is confirmed.
    /// </summary>
    public bool EmailConfirmed { get; set; }

    /// <summary>
    /// Whether phone number is confirmed.
    /// </summary>
    public bool PhoneNumberConfirmed { get; set; }

    /// <summary>
    /// User's roles.
    /// </summary>
    public IList<string> Roles { get; set; } = new List<string>();

    /// <summary>
    /// External login providers linked to this account.
    /// </summary>
    public IList<string> LinkedProviders { get; set; } = new List<string>();
}
