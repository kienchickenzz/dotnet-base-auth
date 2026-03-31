/**
 * ApplicationUser extends IdentityUser with custom properties.
 *
 * <p>Includes audit trail and soft delete support.</p>
 */
namespace AuthService.Identity.Entities;

using Microsoft.AspNetCore.Identity;

using AuthService.Domain.Common;


/// <summary>
/// Application user entity with audit and soft delete support.
/// </summary>
public class ApplicationUser : IdentityUser<Guid>, IAuditableEntity
{
    /// <summary>
    /// User's first name.
    /// </summary>
    public string? FirstName { get; set; }

    /// <summary>
    /// User's last name.
    /// </summary>
    public string? LastName { get; set; }

    /// <summary>
    /// URL to user's profile image.
    /// </summary>
    public string? ImageUrl { get; set; }

    /// <summary>
    /// Indicates if user account is active.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Current refresh token for JWT authentication.
    /// </summary>
    public string? RefreshToken { get; set; }

    /// <summary>
    /// Expiry time of the refresh token.
    /// </summary>
    public DateTime RefreshTokenExpiryTime { get; set; }

    // IAuditableEntity
    public Guid CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }
    public Guid? LastModifiedBy { get; set; }
    public DateTime? LastModifiedOn { get; set; }

    // ISoftDelete (inherited from IAuditableEntity)
    public DateTime? DeletedOn { get; set; }
    public Guid? DeletedBy { get; set; }
}
