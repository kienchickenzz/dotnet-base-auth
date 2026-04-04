/**
 * JwtCookieMiddleware reads JWT from cookie and populates HttpContext.User.
 *
 * <p>Enables JWT-based authentication for MVC web applications.</p>
 */
namespace AuthService.Identity.Middlewares;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

using AuthService.Identity.Settings;

using Microsoft.IdentityModel.Tokens;

/// <summary>
/// Middleware to authenticate requests using JWT stored in cookie.
/// </summary>
public class JwtCookieMiddleware : IMiddleware
{
    private readonly JwtSettings _jwtSettings;

    /// <summary>
    /// Cookie name for access token.
    /// </summary>
    public const string AccessTokenCookieName = "access_token";

    /// <summary>
    /// Cookie name for refresh token.
    /// </summary>
    public const string RefreshTokenCookieName = "refresh_token";

    /// <summary>
    /// Initializes middleware with JWT settings.
    /// </summary>
    public JwtCookieMiddleware(IOptions<JwtSettings> jwtSettings)
    {
        _jwtSettings = jwtSettings.Value;
    }

    /// <summary>
    /// Reads JWT from cookie, validates it, and populates HttpContext.User.
    /// </summary>
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var token = context.Request.Cookies[AccessTokenCookieName];

        if (!string.IsNullOrEmpty(token))
        {
            try
            {
                var principal = _ValidateToken(token, _jwtSettings);
                if (principal != null)
                {
                    context.User = principal;
                }
            }
            catch
            {
                // Token invalid or expired - User remains unauthenticated
                // Could add refresh token logic here
            }
        }

        await next(context);
    }

    /// <summary>
    /// Validates JWT token and returns ClaimsPrincipal.
    /// </summary>
    private static ClaimsPrincipal? _ValidateToken(string token, JwtSettings jwtSettings)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(jwtSettings.Key);

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
        return principal;
    }
}
