/**
 * JwtTokenGenerator implements JWT token generation and validation.
 *
 * <p>Uses HMAC-SHA256 signing with configurable expiration times.</p>
 */
namespace AuthService.Identity.Services;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using AuthService.Application.Features.Identities.Authentication.Services;
using AuthService.Domain.Constants.Identity;
using AuthService.Identity.Settings;


/// <summary>
/// JWT token generator using HMAC-SHA256 signing algorithm.
/// </summary>
internal sealed class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly JwtSettings _jwtSettings;

    public JwtTokenGenerator(IOptions<JwtSettings> jwtSettings)
    {
        _jwtSettings = jwtSettings.Value;
    }

    /// <inheritdoc />
    public string GenerateAccessToken(
        Guid userId,
        string email,
        string? firstName,
        string? lastName,
        string? phoneNumber,
        string? imageUrl,
        string ipAddress)
    {
        var claims = BuildClaims(userId, email, firstName, lastName, phoneNumber, imageUrl, ipAddress);
        var signingCredentials = GetSigningCredentials();

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.TokenExpirationInMinutes),
            signingCredentials: signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <inheritdoc />
    public string GenerateRefreshToken()
    {
        var randomBytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    /// <inheritdoc />
    public DateTime GetRefreshTokenExpiryTime()
    {
        return DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationInDays);
    }

    /// <inheritdoc />
    public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key)),
            ValidateIssuer = false,
            ValidateAudience = false,
            RoleClaimType = ClaimTypes.Role,
            ClockSkew = TimeSpan.Zero,
            ValidateLifetime = false
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        try
        {
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(
                    SecurityAlgorithms.HmacSha256,
                    StringComparison.InvariantCultureIgnoreCase))
            {
                return null;
            }

            return principal;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Builds claims for the JWT token.
    /// </summary>
    private static IEnumerable<Claim> BuildClaims(
        Guid userId,
        string email,
        string? firstName,
        string? lastName,
        string? phoneNumber,
        string? imageUrl,
        string ipAddress)
    {
        return new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new(ClaimTypes.Email, email),
            new(Claims.Fullname, $"{firstName} {lastName}".Trim()),
            new(ClaimTypes.Name, firstName ?? string.Empty),
            new(ClaimTypes.Surname, lastName ?? string.Empty),
            new(Claims.IpAddress, ipAddress),
            new(Claims.ImageUrl, imageUrl ?? string.Empty),
            new(ClaimTypes.MobilePhone, phoneNumber ?? string.Empty)
        };
    }

    /// <summary>
    /// Gets signing credentials using the configured secret key.
    /// </summary>
    private SigningCredentials GetSigningCredentials()
    {
        var secret = Encoding.UTF8.GetBytes(_jwtSettings.Key);
        return new SigningCredentials(
            new SymmetricSecurityKey(secret),
            SecurityAlgorithms.HmacSha256);
    }
}
