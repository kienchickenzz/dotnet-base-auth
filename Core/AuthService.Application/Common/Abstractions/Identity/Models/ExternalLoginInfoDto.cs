/**
 * ExternalLoginInfoDto contains external login provider information.
 *
 * <p>Used to transfer external auth data between layers.</p>
 */
namespace AuthService.Application.Common.Abstractions.Identity.Models;


/// <summary>
/// DTO for external login information from OAuth providers.
/// </summary>
/// <param name="LoginProvider">External provider name (Google, Facebook, etc.).</param>
/// <param name="ProviderKey">User's unique key from the provider.</param>
/// <param name="ProviderDisplayName">Display name of the provider.</param>
/// <param name="Email">User's email from the provider.</param>
/// <param name="FirstName">User's first name from the provider.</param>
/// <param name="LastName">User's last name from the provider.</param>
public sealed record ExternalLoginInfoDto(
    string LoginProvider, // TODO: Consider using enum or constants for known providers to avoid typos
    string ProviderKey, // TODO: Trường này để làm gì??
    string ProviderDisplayName,
    string? Email,
    string? FirstName,
    string? LastName);
// TODO: Đẩy lên Domain với ExternalLoginCallbackResponse??