/**
 * ExternalLoginController handles OAuth authentication flow.
 *
 * <p>Shared controller - redirects based on user role after OAuth.</p>
 */
namespace AuthService.Web.Controllers.Auth;

using MediatR;
using Microsoft.AspNetCore.Mvc;

using AuthService.Application.Common.Abstractions.Identity;
using AuthService.Application.Features.Identities.Authentication.Commands.ExternalLogin;
using AuthService.Identity.Middlewares;
using AuthService.Web.Models.Auth;


/// <summary>
/// Shared controller for external login (OAuth) functionality.
/// </summary>
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
        RedirectToAction("Login", "Login");

    /// <summary>
    /// Initiates external authentication challenge.
    /// </summary>
    [HttpPost]
    public IActionResult Challenge(string provider, string? returnUrl = null)
    {
        var redirectUrl = Url.Action(nameof(Callback), "ExternalLogin", new { returnUrl });
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
            return RedirectToAction("Login", "Login", new { returnUrl });
        }

        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var command = new ExternalLoginCallbackCommand(ipAddress);
        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            TempData["Error"] = result.Error.Name;
            return RedirectToAction("Login", "Login", new { returnUrl });
        }

        var response = result.Value;

        // Existing user - set cookies and redirect by role
        if (response.IsExistingUser && response.Token != null)
        {
            _SetTokenCookies(response.Token.Token, response.Token.RefreshToken);
            _logger.LogInformation("User logged in via external provider.");
            return _RedirectAfterLogin(returnUrl);
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

            return View("~/Views/Auth/ExternalLogin.cshtml", model);
        }

        TempData["Error"] = "External login failed.";
        return RedirectToAction("Login", "Login", new { returnUrl });
    }

    /// <summary>
    /// Confirms external login and creates user account.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Confirmation(ExternalLoginViewModel model)
    {
        model.ReturnUrl ??= Url.Content("~/");

        if (!ModelState.IsValid)
            return View("~/Views/Auth/ExternalLogin.cshtml", model);

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
            return View("~/Views/Auth/ExternalLogin.cshtml", model);
        }

        _SetTokenCookies(result.Value.Token, result.Value.RefreshToken);
        _logger.LogInformation("User {Email} created via external provider.", model.Email);

        // New user from OAuth = Customer (cannot be Admin via OAuth)
        return RedirectToAction("Index", "Profile", new { area = "Customer" });
    }

    /// <summary>
    /// Redirects user based on returnUrl or role.
    /// </summary>
    private IActionResult _RedirectAfterLogin(string? returnUrl)
    {
        if (!string.IsNullOrEmpty(returnUrl) && returnUrl != "/" && returnUrl != "~/")
            return LocalRedirect(returnUrl);

        return _RedirectByRole();
    }

    /// <summary>
    /// Redirects user to appropriate area based on their role.
    /// </summary>
    private IActionResult _RedirectByRole()
    {
        if (User.IsInRole("Admin"))
            return RedirectToAction("Index", "Dashboard", new { area = "Admin" });

        return RedirectToAction("Index", "Profile", new { area = "Customer" });
    }

    /// <summary>
    /// Sets JWT tokens in HttpOnly cookies.
    /// </summary>
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
