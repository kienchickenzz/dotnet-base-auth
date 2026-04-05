/**
 * UserExternalLoginDto represents a linked external login provider.
 *
 * <p>Used to display which OAuth providers are linked to user's account.</p>
 */
namespace AuthService.Application.Common.Abstractions.Identity.Models;


/// <summary>
/// External login provider linked to user account.
/// </summary>
public sealed record UserExternalLoginDto
{
    /// <summary>
    /// Provider name (e.g., "Google", "Facebook").
    /// </summary>
    public string LoginProvider { get; init; } = default!;

    /// <summary>
    /// Provider's unique key for this user.
    /// </summary>
    public string ProviderKey { get; init; } = default!;

    /// <summary>
    /// Display name of the provider.
    /// </summary>
    public string ProviderDisplayName { get; init; } = default!;
}
