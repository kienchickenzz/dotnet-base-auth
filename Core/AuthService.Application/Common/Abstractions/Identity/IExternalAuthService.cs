/**
 * IExternalAuthService defines contract for external authentication operations.
 *
 * <p>Handles OAuth/external provider authentication flows.</p>
 */
namespace AuthService.Application.Common.Abstractions.Identity;

using Microsoft.AspNetCore.Authentication;

using AuthService.Application.Common.Abstractions.Identity.Models;
using AuthService.Domain.Common;


/// <summary>
/// Service for external authentication (OAuth) operations.
/// </summary>
/// <remarks>
/// This service handles two distinct data sources:
/// <list type="bullet">
///   <item>OAuth Provider (Google, Facebook): User info from consent screen</item>
///   <item>Local Database: User profile stored in our system</item>
/// </list>
/// </remarks>
public interface IExternalAuthService
{
    /// <summary>
    /// Gets available external authentication providers.
    /// </summary>
    Task<IList<AuthenticationScheme>> GetExternalAuthSchemesAsync();

    /// <summary>
    /// Configures external authentication challenge properties.
    /// </summary>
    /// <param name="provider">External provider name (Google, Facebook, etc.).</param>
    /// <param name="redirectUrl">URL to redirect after authentication.</param>
    AuthenticationProperties ConfigureExternalAuthProperties(string provider, string redirectUrl);

    /// <summary>
    /// Gets external login info FROM OAUTH PROVIDER (not from database).
    /// </summary>
    /// <remarks>
    /// <para>
    /// DATA SOURCE: OAuth Provider (Google, Facebook, etc.)
    /// </para>
    /// <para>
    /// This reads the temporary authentication cookie set by ASP.NET Core OAuth middleware
    /// after user consents on the provider's page. Returns info that THE PROVIDER knows
    /// about the user (email, name from Google profile).
    /// </para>
    /// <para>
    /// Use this to: Identify WHO is logging in and get their ProviderKey for database lookup.
    /// </para>
    /// </remarks>
    /// <returns>External login info from OAuth provider (ProviderKey, email, name from provider).</returns>
    Task<Result<ExternalLoginInfoDto>> GetExternalLoginInfoAsync();

    /// <summary>
    /// Finds existing user FROM LOCAL DATABASE by external login credentials.
    /// </summary>
    /// <remarks>
    /// <para>
    /// DATA SOURCE: Local Database (AspNetUserLogins + AspNetUsers tables)
    /// </para>
    /// <para>
    /// This queries our database to find a user who previously registered with this
    /// external provider. Returns OUR stored profile (which may differ from provider's data,
    /// e.g., admin may have updated email, added ImageUrl, assigned roles).
    /// </para>
    /// <para>
    /// Use this to: Get full user profile for JWT generation after confirming user exists.
    /// </para>
    /// </remarks>
    /// <param name="loginProvider">External provider name (e.g., "Google").</param>
    /// <param name="providerKey">Provider's unique user ID (NOT email).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>User profile from database, or error if user not found.</returns>
    Task<Result<UserDto>> ExternalLoginAsync(
        string loginProvider,
        string providerKey,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates new user in database and links external login.
    /// </summary>
    /// <remarks>
    /// Called when GetExternalLoginInfoAsync succeeds but ExternalLoginAsync fails
    /// (user authenticated with provider but doesn't exist in our database yet).
    /// </remarks>
    /// <param name="user">User data (from confirmation form).</param>
    /// <param name="externalLogin">External login info (from GetExternalLoginInfoAsync).</param>
    /// <param name="origin">Origin URL for confirmation emails.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Created user's Id.</returns>
    Task<Result<Guid>> CreateUserWithExternalLoginAsync(
        CreateUserDto user,
        ExternalLoginInfoDto externalLogin,
        string origin,
        CancellationToken cancellationToken = default);
}
