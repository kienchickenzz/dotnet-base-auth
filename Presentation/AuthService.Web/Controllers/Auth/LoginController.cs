/**
 * LoginController handles user authentication using JWT.
 *
 * <p>Shared controller - redirects to appropriate area based on user role.</p>
 */

using AuthService.Application.Features.Identities.Authentication.Commands.Login;
using AuthService.Identity.Middlewares;
using AuthService.Web.Models.Auth;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Web.Controllers.Auth;

/// <summary>
/// Shared controller for user login functionality using JWT.
/// </summary>
public class LoginController : Controller
{
    private readonly IMediator _mediator;
    private readonly ILogger<LoginController> _logger;

    /// <summary>
    /// Initializes LoginController with required dependencies.
    /// </summary>
    public LoginController(
        IMediator mediator,
        ILogger<LoginController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Displays the login form.
    /// </summary>
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        // Redirect by role if already logged in
        if (User.Identity?.IsAuthenticated == true)
            return _RedirectByRole();

        var model = new LoginViewModel
        {
            ReturnUrl = returnUrl ?? Url.Content("~/")
        };

        return View("~/Views/Auth/Login.cshtml", model);
    }

    /// <summary>
    /// Processes the login form submission.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        model.ReturnUrl ??= Url.Content("~/");

        if (!ModelState.IsValid)
            return View("~/Views/Auth/Login.cshtml", model);

        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        var command = new LoginCommand(
            model.Email,
            model.Password,
            ipAddress);

        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            ModelState.AddModelError(string.Empty, result.Error.Name);
            return View("~/Views/Auth/Login.cshtml", model);
        }

        var tokenResponse = result.Value;

        _SetTokenCookies(tokenResponse.Token, tokenResponse.RefreshToken, model.RememberMe);

        _logger.LogInformation("User {Email} logged in.", model.Email);

        return _RedirectAfterLogin(model.ReturnUrl);
    }

    /// <summary>
    /// Logs out the current user by clearing cookies.
    /// </summary>
    [HttpPost]
    public IActionResult Logout()
    {
        Response.Cookies.Delete(JwtCookieMiddleware.AccessTokenCookieName);
        Response.Cookies.Delete(JwtCookieMiddleware.RefreshTokenCookieName);

        _logger.LogInformation("User logged out.");

        return RedirectToAction("Index", "Home", new { area = "" });
    }

    /// <summary>
    /// Redirects user based on returnUrl or role.
    /// </summary>
    private IActionResult _RedirectAfterLogin(string? returnUrl)
    {
        // If valid returnUrl exists, redirect there
        if (!string.IsNullOrEmpty(returnUrl) && returnUrl != "/" && returnUrl != "~/")
            return LocalRedirect(returnUrl);

        // No returnUrl - redirect based on role
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
    private void _SetTokenCookies(string accessToken, string refreshToken, bool rememberMe)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Lax,
            Path = "/",
            Expires = rememberMe ? DateTimeOffset.UtcNow.AddDays(7) : null
        };

        Response.Cookies.Append(
            JwtCookieMiddleware.AccessTokenCookieName,
            accessToken,
            cookieOptions);

        Response.Cookies.Append(
            JwtCookieMiddleware.RefreshTokenCookieName,
            refreshToken,
            cookieOptions);
    }
}
