/**
 * UpdateRoleCommand updates an existing role.
 *
 * <p>Processed by UpdateRoleCommandHandler.</p>
 */
namespace AuthService.Application.Features.Identities.Roles.Commands.UpdateRole;

using AuthService.Application.Common.Messaging;


/// <summary>
/// Command to update a role.
/// </summary>
/// <param name="Id">Role's unique identifier.</param>
/// <param name="Name">Role name.</param>
/// <param name="Description">Role description (optional).</param>
public sealed record UpdateRoleCommand(
    Guid Id,
    string Name,
    string? Description) : ICommand<Guid>;
