/**
 * UsersController handles admin user management API endpoints.
 *
 * <p>Uses MediatR to dispatch commands and queries. Requires admin permissions.</p>
 */
namespace AuthService.Api.Controllers.Identity;

using MediatR;

using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;

using AuthService.Api.Extensions;
using AuthService.Application.Common.Abstractions.Identity.Models;
using AuthService.Application.Features.Identities.Users.Commands.AssignRoles;
using AuthService.Application.Features.Identities.Users.Commands.CreateUser;
using AuthService.Application.Features.Identities.Users.Commands.DeleteUser;
using AuthService.Application.Features.Identities.Users.Commands.ToggleUserStatus;
using AuthService.Application.Features.Identities.Users.Commands.UpdateUser;
using AuthService.Application.Features.Identities.Users.Queries.GetUserById;
using AuthService.Application.Features.Identities.Users.Queries.GetUserRoles;
using AuthService.Application.Features.Identities.Users.Queries.GetUsers;
using AuthService.Application.Features.Identities.Roles;
using AuthService.Identity.Auth.Permissions;


/// <summary>
/// Admin user management endpoints.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly ISender _sender;

    public UsersController(ISender sender)
    {
        _sender = sender ?? throw new ArgumentNullException(nameof(sender));
    }

    /// <summary>
    /// Gets all users.
    /// </summary>
    [HttpGet]
    [MustHavePermission(Actions.View, Resource.Users)]
    public async Task<ActionResult<List<UserDto>>> GetListAsync(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetUsersQuery(), cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.Error.ToBadRequest();
    }

    /// <summary>
    /// Gets user by Id.
    /// </summary>
    [HttpGet("{id:guid}")]
    [MustHavePermission(Actions.View, Resource.Users)]
    public async Task<ActionResult<UserDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetUserByIdQuery(id), cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.Error.ToNotFound();
    }

    /// <summary>
    /// Gets roles for a user.
    /// </summary>
    [HttpGet("{id:guid}/roles")]
    [MustHavePermission(Actions.View, Resource.UserRoles)]
    public async Task<ActionResult<List<UserRoleDto>>> GetRolesAsync(Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetUserRolesQuery(id), cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.Error.ToNotFound();
    }

    /// <summary>
    /// Assigns roles to a user.
    /// </summary>
    [HttpPost("{id:guid}/roles")]
    [MustHavePermission(Actions.Update, Resource.UserRoles)]
    public async Task<ActionResult> AssignRolesAsync(
        Guid id,
        List<UserRoleDto> roles,
        CancellationToken cancellationToken)
    {
        var command = new AssignUserRolesCommand(id, roles);
        var result = await _sender.Send(command, cancellationToken);
        return result.IsSuccess ? NoContent() : result.Error.ToBadRequest();
    }

    /// <summary>
    /// Creates a new user (admin).
    /// </summary>
    [HttpPost]
    [MustHavePermission(Actions.Create, Resource.Users)]
    public async Task<ActionResult<Guid>> CreateAsync(
        CreateUserCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        if (!result.IsSuccess)
        {
            return result.Error.ToBadRequest();
        }

        return Created($"/api/users/{result.Value}", result.Value);
    }

    /// <summary>
    /// Updates user profile.
    /// </summary>
    [HttpPut("{id:guid}")]
    [MustHavePermission(Actions.Update, Resource.Users)]
    public async Task<ActionResult> UpdateAsync(
        Guid id,
        UpdateUserCommand command,
        CancellationToken cancellationToken)
    {
        if (id != command.Id)
        {
            return BadRequest("Id mismatch.");
        }

        var result = await _sender.Send(command, cancellationToken);
        return result.IsSuccess ? NoContent() : result.Error.ToBadRequest();
    }

    /// <summary>
    /// Toggles user active status.
    /// </summary>
    [HttpPost("{id:guid}/toggle-status")]
    [MustHavePermission(Actions.Update, Resource.Users)]
    public async Task<ActionResult> ToggleStatusAsync(
        Guid id,
        [FromBody] bool activate,
        CancellationToken cancellationToken)
    {
        var command = new ToggleUserStatusCommand(id, activate);
        var result = await _sender.Send(command, cancellationToken);
        return result.IsSuccess ? NoContent() : result.Error.ToBadRequest();
    }

    /// <summary>
    /// Soft-deletes a user.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [MustHavePermission(Actions.Delete, Resource.Users)]
    public async Task<ActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new DeleteUserCommand(id), cancellationToken);
        return result.IsSuccess ? NoContent() : result.Error.ToNotFound();
    }
}
