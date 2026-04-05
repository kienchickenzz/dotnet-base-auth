/**
 * ExternalLoginController handles OAuth authentication flow.
 *
 * <p>Manages challenge, callback, and confirmation for external providers.</p>
 */
namespace AuthService.Web.Areas.Customer.Features.Auth.ExternalLogin.Controllers;

using MediatR;
using Microsoft.AspNetCore.Mvc;

using AuthService.Application.Common.Abstractions.Identity;
using AuthService.Application.Features.Identities.Authentication.Commands.ExternalLogin;
using AuthService.Identity.Middlewares;
using AuthService.Web.Areas.Customer.Features.Auth.ExternalLogin.Models;


/// <summary>
/// Controller for external login (OAuth) functionality.
/// </summary>
[Area("Customer")]
public class ExternalLoginController : Controller
{
    private readonly IMediator _mediator;
    private readonly IExternalAuthService _externalAuthService;
    private readonly ILogger<ExternalLoginController> _logger;

    /// <summary>
    /// Initializes ExternalLoginController with required dependencies.
    /// </summary>
    public ExternalLoginController(
        IMediator mediator,
        IExternalAuthService externalAuthService,
        ILogger<ExternalLoginController> logger)
    {
        _mediator = mediator;
        _externalAuthService = externalAuthService;
        _logger = logger;
    }

    /// <summary>
    /// Redirects to login page (external login requires POST).
    /// </summary>
    [HttpGet]
    public IActionResult Index() =>
        RedirectToAction("Login", "Login", new { area = "Customer" });

    /// <summary>
    /// Initiates external authentication challenge.
    /// </summary>
    [HttpPost]
    public IActionResult Challenge(string provider, string? returnUrl = null)
    {
        var redirectUrl = Url.Action(nameof(Callback), "ExternalLogin", new { area = "Customer", returnUrl });
        var properties = _externalAuthService.ConfigureExternalAuthProperties(provider, redirectUrl!);
        return new ChallengeResult(provider, properties);
    }

    /// <summary>
    /// Handles callback from external provider.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Callback(string? returnUrl = null, string? remoteError = null)
    {
        returnUrl ??= Url.Content("~/");

        if (remoteError != null)
        {
            TempData["Error"] = $"Error from external provider: {remoteError}";
            return RedirectToAction("Login", "Login", new { area = "Customer", returnUrl });
        }

        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var command = new ExternalLoginCallbackCommand(ipAddress);
        var result = await _mediator.Send(command);

        // TODO: Trường hợp user hủy login trên provider thì sao? remoteError có được set không?
        if (result.IsFailure)
        {
            TempData["Error"] = result.Error.Name;
            return RedirectToAction("Login", "Login", new { area = "Customer", returnUrl });
        }

        var response = result.Value;

        // Existing user - set cookies and redirect
        if (response.IsExistingUser && response.Token != null)
        {
            // TODO: Redirect về trang home đã đăng nhập thay vì landing page
            _SetTokenCookies(response.Token.Token, response.Token.RefreshToken);
            _logger.LogInformation("User logged in via external provider.");
            return LocalRedirect(returnUrl);
        }

        // New user - show confirmation form
        if (response.ExternalLoginInfo != null)
        {
            var model = new ExternalLoginViewModel
            {
                Email = response.ExternalLoginInfo.Email ?? string.Empty,
                FirstName = response.ExternalLoginInfo.FirstName ?? string.Empty,
                LastName = response.ExternalLoginInfo.LastName ?? string.Empty,
                ProviderDisplayName = response.ExternalLoginInfo.ProviderDisplayName,
                ReturnUrl = returnUrl
            };

            return View("ExternalLogin", model);
        }

        TempData["Error"] = "External login failed.";
        return RedirectToAction("Login", "Login", new { area = "Customer", returnUrl });
    }

    /// <summary>
    /// Confirms external login and creates user account.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Confirmation(ExternalLoginViewModel model)
    {
        model.ReturnUrl ??= Url.Content("~/");

        if (!ModelState.IsValid)
            return View("ExternalLogin", model);

        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var origin = $"{Request.Scheme}://{Request.Host.Value}";

        var command = new ExternalLoginConfirmationCommand(
            model.Email,
            model.FirstName,
            model.LastName,
            model.PhoneNumber,
            model.Password,
            ipAddress,
            origin);

        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            ModelState.AddModelError(string.Empty, result.Error.Name);
            return View("ExternalLogin", model);
        }

        // Set cookies and redirect
        _SetTokenCookies(result.Value.Token, result.Value.RefreshToken);
        _logger.LogInformation("User {Email} created via external provider.", model.Email);

        // TODO: Redirect về trang home đã đăng nhập thay vì landing page
        return LocalRedirect(model.ReturnUrl);
    }

    /// <summary>
    /// Sets JWT tokens in HttpOnly cookies.
    /// </summary>
    /// <remarks>
    /// Cookie settings explained:
    /// - HttpOnly: Prevents JavaScript access (XSS protection)
    /// - Secure: Cookie only sent over HTTPS
    /// - Path="/": Cookie available for all paths (default is current request path)
    /// - SameSite=Lax: IMPORTANT for OAuth flows!
    ///
    ///   Why not Strict? After OAuth redirect (Google → callback → home), the browser
    ///   considers this a "cross-site initiated" navigation. With SameSite=Strict,
    ///   cookies are NOT sent on the first redirect, causing the navbar to show
    ///   unauthenticated state even though login succeeded.
    ///
    ///   Lax sends cookies on top-level GET navigations/redirects while still
    ///   protecting against CSRF on cross-site POST requests.
    /// </remarks>
    private void _SetTokenCookies(string accessToken, string refreshToken)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Lax,
            Path = "/"
        };

        Response.Cookies.Append(JwtCookieMiddleware.AccessTokenCookieName, accessToken, cookieOptions);
        Response.Cookies.Append(JwtCookieMiddleware.RefreshTokenCookieName, refreshToken, cookieOptions);
    }
}
