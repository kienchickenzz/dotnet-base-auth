/**
 * RolePermissionsViewModel for managing role permissions.
 *
 * <p>Groups permissions by resource for easier management.</p>
 */
namespace AuthService.Web.Areas.Admin.Features.Role.Models;


/// <summary>
/// ViewModel for role permissions management page.
/// </summary>
public class RolePermissionsViewModel
{
    /// <summary>
    /// Role ID.
    /// </summary>
    public Guid RoleId { get; set; }

    /// <summary>
    /// Role name.
    /// </summary>
    public string RoleName { get; set; } = string.Empty;

    /// <summary>
    /// True if this is a default role.
    /// </summary>
    public bool IsDefault { get; set; }

    /// <summary>
    /// Grouped permissions by resource.
    /// </summary>
    public List<PermissionGroupViewModel> PermissionGroups { get; set; } = [];
}


/// <summary>
/// Group of permissions for a resource.
/// </summary>
public class PermissionGroupViewModel
{
    /// <summary>
    /// Resource name (Users, Roles, etc.).
    /// </summary>
    public string Resource { get; set; } = string.Empty;

    /// <summary>
    /// Permissions in this group.
    /// </summary>
    public List<PermissionItemViewModel> Permissions { get; set; } = [];
}


/// <summary>
/// Single permission item.
/// </summary>
public class PermissionItemViewModel
{
    /// <summary>
    /// Permission name (Permissions.Users.View).
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Display name (View Users).
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// True if permission is assigned to the role.
    /// </summary>
    public bool IsAssigned { get; set; }
}
