/**
 * RoleIndexViewModel displays list of roles.
 *
 * <p>Used in role management index page.</p>
 */
namespace AuthService.Web.Areas.Admin.Features.Role.Models;


/// <summary>
/// ViewModel for role list page.
/// </summary>
public class RoleIndexViewModel
{
    /// <summary>
    /// List of roles to display.
    /// </summary>
    public IReadOnlyList<RoleItemViewModel> Roles { get; set; } = [];
}


/// <summary>
/// Single role item in the list.
/// </summary>
public class RoleItemViewModel
{
    /// <summary>
    /// Role unique identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Role name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Role description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// True if this is a default role (Admin, Customer).
    /// </summary>
    public bool IsDefault { get; set; }

    /// <summary>
    /// Number of permissions assigned to this role.
    /// </summary>
    public int PermissionCount { get; set; }
}
