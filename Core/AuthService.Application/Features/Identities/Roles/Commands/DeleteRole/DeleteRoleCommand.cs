/**
 * DeleteRoleCommand deletes a role.
 *
 * <p>Processed by DeleteRoleCommandHandler.</p>
 */
namespace AuthService.Application.Features.Identities.Roles.Commands.DeleteRole;

using AuthService.Application.Common.Messaging;


/// <summary>
/// Command to delete a role.
/// </summary>
/// <param name="RoleId">Role's unique identifier.</param>
public sealed record DeleteRoleCommand(Guid RoleId) : ICommand;
