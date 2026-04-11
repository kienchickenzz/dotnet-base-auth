/**
 * RoleFormViewModel for create/edit role forms.
 *
 * <p>Contains validation attributes for role form fields.</p>
 */
namespace AuthService.Web.Areas.Admin.Features.Role.Models;

using System.ComponentModel.DataAnnotations;


/// <summary>
/// ViewModel for role create/edit form.
/// </summary>
public class RoleFormViewModel
{
    /// <summary>
    /// Role ID (null for create, populated for edit).
    /// </summary>
    public Guid? Id { get; set; }

    /// <summary>
    /// Role name.
    /// </summary>
    [Required(ErrorMessage = "Role name is required")]
    [StringLength(50, ErrorMessage = "Role name cannot exceed 50 characters")]
    [Display(Name = "Role Name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Role description.
    /// </summary>
    [StringLength(200, ErrorMessage = "Description cannot exceed 200 characters")]
    [Display(Name = "Description")]
    public string? Description { get; set; }

    /// <summary>
    /// True if editing existing role.
    /// </summary>
    public bool IsEdit => Id.HasValue;

    /// <summary>
    /// True if this is a default role (cannot edit name).
    /// </summary>
    public bool IsDefault { get; set; }
}
