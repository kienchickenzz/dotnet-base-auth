/**
 * ApplicationRole extends IdentityRole with custom properties.
 *
 * <p>Includes audit trail and soft delete support.</p>
 */
namespace AuthService.Identity.Entities;

using Microsoft.AspNetCore.Identity;

using AuthService.Domain.Common;


/// <summary>
/// Application role entity with audit and soft delete support.
/// </summary>
public class ApplicationRole : IdentityRole<Guid>, IAuditableEntity
{
    /// <summary>
    /// Role description.
    /// </summary>
    public string? Description { get; set; }

    // IAuditableEntity
    public Guid CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }
    public Guid? LastModifiedBy { get; set; }
    public DateTime? LastModifiedOn { get; set; }

    // ISoftDelete (inherited from IAuditableEntity)
    public DateTime? DeletedOn { get; set; }
    public Guid? DeletedBy { get; set; }

    /// <summary>
    /// Default constructor for EF Core.
    /// </summary>
    public ApplicationRole() : base() { }

    /// <summary>
    /// Creates a new role with name and optional description.
    /// </summary>
    public ApplicationRole(string name, string? description = null)
        : base(name)
    {
        Description = description;
        NormalizedName = name.ToUpperInvariant();
    }
}