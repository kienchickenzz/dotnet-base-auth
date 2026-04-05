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
    /// Gets external login info from OAuth callback.
    /// </summary>
    Task<Result<ExternalLoginInfoDto>> GetExternalLoginInfoAsync();

    /// <summary>
    /// Attempts to sign in user with external login.
    /// </summary>
    /// <param name="loginProvider">External provider name.</param>
    /// <param name="providerKey">User's key from provider.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result with UserDto if exists, or error if new user.</returns>
    Task<Result<UserDto>> ExternalLoginAsync(
        string loginProvider,
        string providerKey,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates user and links external login.
    /// </summary>
    /// <param name="user">User data.</param>
    /// <param name="externalLogin">External login info.</param>
    /// <param name="origin">Origin URL for confirmation emails.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result with created user Id.</returns>
    Task<Result<Guid>> CreateUserWithExternalLoginAsync(
        CreateUserDto user,
        ExternalLoginInfoDto externalLogin,
        string origin,
        CancellationToken cancellationToken = default);
}
