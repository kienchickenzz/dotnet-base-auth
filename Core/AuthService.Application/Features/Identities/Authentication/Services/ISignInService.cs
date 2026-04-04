/**
 * ISignInService defines contract for cookie-based sign-in operations.
 *
 * <p>Only handles cookie/session creation, NOT credential validation.</p>
 */
namespace AuthService.Application.Features.Identities.Authentication.Services;

using Microsoft.AspNetCore.Authentication;

/// <summary>
/// Service for cookie-based sign-in operations (Web MVC).
/// Does NOT validate credentials - use IAuthenticationService for that.
/// </summary>
public interface ISignInService
{
    /// <summary>
    /// Creates authentication cookie for validated user.
    /// </summary>
    /// <param name="userId">Validated user's ID.</param>
    /// <param name="rememberMe">Whether to persist cookie.</param>
    Task SignInAsync(Guid userId, bool rememberMe);

    /// <summary>
    /// Signs out current user and clears cookies.
    /// </summary>
    Task SignOutAsync();

    /// <summary>
    /// Clears external authentication cookie.
    /// </summary>
    Task SignOutExternalSchemeAsync();

    /// <summary>
    /// Gets available external authentication providers.
    /// </summary>
    Task<IList<AuthenticationScheme>> GetExternalAuthenticationSchemesAsync();
}
