/**
 * ApplicationRoleClaim extends IdentityRoleClaim with audit properties.
 *
 * <p>Includes audit trail and soft delete support.</p>
 */
namespace AuthService.Identity.Entities;

using Microsoft.AspNetCore.Identity;

using AuthService.Domain.Common;


/// <summary>
/// Application role claim entity with audit and soft delete support.
/// </summary>
public class ApplicationRoleClaim : IdentityRoleClaim<Guid>, IAuditableEntity
{
    // IAuditableEntity
    public Guid CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }
    public Guid? LastModifiedBy { get; set; }
    public DateTime? LastModifiedOn { get; set; }

    // ISoftDelete (inherited from IAuditableEntity)
    public DateTime? DeletedOn { get; set; }
    public Guid? DeletedBy { get; set; }
}