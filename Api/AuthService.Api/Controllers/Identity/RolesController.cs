/**
 * RolesController handles role management API endpoints.
 *
 * <p>Uses MediatR to dispatch commands and queries.</p>
 */
namespace AuthService.Api.Controllers.Identity;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using AuthService.Application.Common.Abstractions.Identity.Models;
using AuthService.Application.Features.Identities.Roles.Commands.CreateRole;
using AuthService.Application.Features.Identities.Roles.Commands.DeleteRole;
using AuthService.Application.Features.Identities.Roles.Commands.UpdateRole;
using AuthService.Application.Features.Identities.Roles.Commands.UpdateRolePermissions;
using AuthService.Application.Features.Identities.Roles.Queries.GetRoleById;
using AuthService.Application.Features.Identities.Roles.Queries.GetRoles;
using AuthService.Application.Features.Identities.Roles.Queries.GetRoleWithPermissions;
using AuthService.Application.Features.Identities.Roles;
using AuthService.Identity.Auth.Permissions;


/// <summary>
/// Role management endpoints.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class RolesController : ControllerBase
{
    private readonly ISender _sender;

    public RolesController(ISender sender)
    {
        _sender = sender ?? throw new ArgumentNullException(nameof(sender));
    }

    /// <summary>
    /// Gets all roles.
    /// </summary>
    [HttpGet]
    [MustHavePermission(Action.View, Resource.Roles)]
    public async Task<ActionResult<List<RoleDto>>> GetListAsync(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetRolesQuery(), cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    /// <summary>
    /// Gets role by Id.
    /// </summary>
    [HttpGet("{id:guid}")]
    [MustHavePermission(Action.View, Resource.Roles)]
    public async Task<ActionResult<RoleDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetRoleByIdQuery(id), cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
    }

    /// <summary>
    /// Gets role by Id with permissions.
    /// </summary>
    [HttpGet("{id:guid}/permissions")]
    [MustHavePermission(Action.View, Resource.RoleClaims)]
    public async Task<ActionResult<RoleDto>> GetByIdWithPermissionsAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetRoleWithPermissionsQuery(id), cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
    }

    /// <summary>
    /// Creates a new role.
    /// </summary>
    [HttpPost]
    [MustHavePermission(Action.Create, Resource.Roles)]
    public async Task<ActionResult<Guid>> CreateAsync(
        CreateRoleCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetByIdAsync), new { id = result.Value }, result.Value)
            : BadRequest(result.Error);
    }

    /// <summary>
    /// Updates an existing role.
    /// </summary>
    [HttpPut("{id:guid}")]
    [MustHavePermission(Action.Update, Resource.Roles)]
    public async Task<ActionResult<Guid>> UpdateAsync(
        Guid id,
        UpdateRoleCommand command,
        CancellationToken cancellationToken)
    {
        if (id != command.Id)
        {
            return BadRequest("Id mismatch.");
        }

        var result = await _sender.Send(command, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    /// <summary>
    /// Updates role permissions.
    /// </summary>
    [HttpPut("{id:guid}/permissions")]
    [MustHavePermission(Action.Update, Resource.RoleClaims)]
    public async Task<ActionResult> UpdatePermissionsAsync(
        Guid id,
        List<string> permissions,
        CancellationToken cancellationToken)
    {
        var command = new UpdateRolePermissionsCommand(id, permissions);
        var result = await _sender.Send(command, cancellationToken);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    /// <summary>
    /// Deletes a role.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [MustHavePermission(Action.Delete, Resource.Roles)]
    public async Task<ActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteRoleCommand(id);
        var result = await _sender.Send(command, cancellationToken);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }
}