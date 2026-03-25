/**
 * AssignUserRolesCommand assigns roles to a user.
 *
 * <p>Processed by AssignUserRolesCommandHandler.</p>
 */
namespace AuthService.Application.Features.Identities.Users.Commands.AssignRoles;

using AuthService.Application.Common.Abstractions.Identity.Models;
using AuthService.Application.Common.Messaging;


/// <summary>
/// Command to assign roles to a user.
/// </summary>
/// <param name="UserId">User's unique identifier.</param>
/// <param name="Roles">List of roles to assign.</param>
public sealed record AssignUserRolesCommand(
    Guid UserId,
    List<UserRoleDto> Roles) : ICommand;
