namespace AuthService.Domain.Constants.Identity;


/// <summary>
/// Custom JWT claim type names used for authentication tokens.
///
/// <para>
/// These constants define claim names for user information that are not available
/// in the standard <see cref="System.Security.Claims.ClaimTypes"/>.
/// Used by AuthService when generating JWT tokens to embed user-specific data.
/// </para>
/// </summary>
public static class Claims
{
    /// <summary>User's full name (FirstName + LastName).</summary>
    public const string Fullname = "fullName";

    /// <summary>User's granted permissions for authorization.</summary>
    public const string Permission = "permission";

    /// <summary>User's profile/avatar image URL.</summary>
    public const string ImageUrl = "imageUrl";

    /// <summary>Client IP address at the time of token generation.</summary>
    public const string IpAddress = "ipAddress";

    /// <summary>Token expiration timestamp (Unix time).</summary>
    public const string Expiration = "exp";
}