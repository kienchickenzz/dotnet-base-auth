/**
 * LoginController handles customer authentication using JWT.
 *
 * <p>Thin controller - delegates to MediatR, stores JWT in HttpOnly cookie.</p>
 */

using AuthService.Application.Features.Identities.Authentication.Commands.Login;
using AuthService.Identity.Middlewares;
using AuthService.Web.Areas.Customer.Features.Auth.Login.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Web.Areas.Customer.Features.Auth.Login.Controllers;

/// <summary>
/// Controller for customer login functionality using JWT.
/// </summary>
[Area("Customer")]
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
        // Redirect to home if already logged in
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Home", new { area = "" });
        }

        var model = new LoginViewModel
        {
            ReturnUrl = returnUrl ?? Url.Content("~/")
        };

        return View(model);
    }

    /// <summary>
    /// Processes the login form submission.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        model.ReturnUrl ??= Url.Content("~/");

        if (!ModelState.IsValid)
            return View(model);

        // Get client IP address
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        // Call LoginCommand (reuse existing JWT infrastructure)
        var command = new LoginCommand(
            model.Email,
            model.Password,
            ipAddress);

        var result = await _mediator.Send(command);

        // Handle failure
        if (result.IsFailure)
        {
            ModelState.AddModelError(string.Empty, result.Error.Name);
            return View(model);
        }

        var tokenResponse = result.Value;

        // Store JWT in HttpOnly cookie
        _SetTokenCookies(tokenResponse.Token, tokenResponse.RefreshToken, model.RememberMe);

        _logger.LogInformation("User {Email} logged in.", model.Email);

        return LocalRedirect(model.ReturnUrl);
    }

    /// <summary>
    /// Logs out the current user by clearing cookies.
    /// </summary>
    [HttpPost]
    public IActionResult Logout()
    {
        // Clear cookies
        Response.Cookies.Delete(JwtCookieMiddleware.AccessTokenCookieName);
        Response.Cookies.Delete(JwtCookieMiddleware.RefreshTokenCookieName);

        _logger.LogInformation("User logged out.");

        return RedirectToAction("Index", "Home", new { area = "" });
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
            SameSite = SameSiteMode.Strict,
            Expires = rememberMe
                ? DateTimeOffset.UtcNow.AddDays(7)
                : null
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
