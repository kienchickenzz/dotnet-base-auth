/**
 * UpdateRolePermissionsCommand updates permissions for a role.
 *
 * <p>Processed by UpdateRolePermissionsCommandHandler.</p>
 */
namespace AuthService.Application.Features.Identities.Roles.Commands.UpdateRolePermissions;

using AuthService.Application.Common.Messaging;


/// <summary>
/// Command to update role permissions.
/// </summary>
/// <param name="RoleId">Role's unique identifier.</param>
/// <param name="Permissions">List of permissions to assign.</param>
public sealed record UpdateRolePermissionsCommand(
    Guid RoleId,
    List<string> Permissions) : ICommand;
