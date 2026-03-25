/**
 * CreateRoleCommand creates a new role.
 *
 * <p>Processed by CreateRoleCommandHandler.</p>
 */
namespace AuthService.Application.Features.Identities.Roles.Commands.CreateRole;

using AuthService.Application.Common.Messaging;


/// <summary>
/// Command to create a new role.
/// </summary>
/// <param name="Name">Role name.</param>
/// <param name="Description">Role description (optional).</param>
public sealed record CreateRoleCommand(
    string Name,
    string? Description) : ICommand<Guid>;
