/**
 * RoleController handles role CRUD operations for Admin area.
 *
 * <p>Inherits Admin role requirement from AdminBaseController.</p>
 */
namespace AuthService.Web.Areas.Admin.Features.Role.Controllers;

using MediatR;
using Microsoft.AspNetCore.Mvc;

using AuthService.Application.Features.Identities.Roles;
using AuthService.Application.Features.Identities.Roles.Commands.CreateRole;
using AuthService.Application.Features.Identities.Roles.Commands.UpdateRole;
using AuthService.Application.Features.Identities.Roles.Commands.DeleteRole;
using AuthService.Application.Features.Identities.Roles.Commands.UpdateRolePermissions;
using AuthService.Application.Features.Identities.Roles.Queries.GetRoles;
using AuthService.Application.Features.Identities.Roles.Queries.GetRoleById;
using AuthService.Application.Features.Identities.Roles.Queries.GetRoleWithPermissions;
using AuthService.Web.Areas.Admin.Features.Role.Models;


/// <summary>
/// Controller for role management in Admin area.
/// </summary>
public class RoleController : AdminBaseController
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Initializes RoleController with MediatR.
    /// </summary>
    public RoleController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Displays list of all roles.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var result = await _mediator.Send(new GetRolesQuery());

        if (result.IsFailure)
        {
            TempData["Error"] = result.Error.Name;
            return View("~/Areas/Admin/Features/Role/Views/Index.cshtml", new RoleIndexViewModel());
        }

        var model = new RoleIndexViewModel
        {
            Roles = result.Value.Select(r => new RoleItemViewModel
            {
                Id = r.Id,
                Name = r.Name,
                Description = r.Description,
                IsDefault = Roles.IsDefault(r.Name),
                PermissionCount = r.Permissions?.Count ?? 0
            }).ToList()
        };

        return View("~/Areas/Admin/Features/Role/Views/Index.cshtml", model);
    }

    /// <summary>
    /// Displays create role form.
    /// </summary>
    [HttpGet]
    public IActionResult Create()
    {
        return View("~/Areas/Admin/Features/Role/Views/Create.cshtml", new RoleFormViewModel());
    }

    /// <summary>
    /// Processes create role form.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create(RoleFormViewModel model)
    {
        if (!ModelState.IsValid)
            return View("~/Areas/Admin/Features/Role/Views/Create.cshtml", model);

        var command = new CreateRoleCommand(model.Name, model.Description);
        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            ModelState.AddModelError(string.Empty, result.Error.Name);
            return View("~/Areas/Admin/Features/Role/Views/Create.cshtml", model);
        }

        TempData["Success"] = $"Role '{model.Name}' created successfully.";
        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// Displays edit role form.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var result = await _mediator.Send(new GetRoleByIdQuery(id));

        if (result.IsFailure)
        {
            TempData["Error"] = result.Error.Name;
            return RedirectToAction(nameof(Index));
        }

        var role = result.Value;
        var model = new RoleFormViewModel
        {
            Id = role.Id,
            Name = role.Name,
            Description = role.Description,
            IsDefault = Roles.IsDefault(role.Name)
        };

        return View("~/Areas/Admin/Features/Role/Views/Edit.cshtml", model);
    }

    /// <summary>
    /// Processes edit role form.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Edit(RoleFormViewModel model)
    {
        if (!ModelState.IsValid)
            return View("~/Areas/Admin/Features/Role/Views/Edit.cshtml", model);

        if (!model.Id.HasValue)
        {
            TempData["Error"] = "Invalid role ID.";
            return RedirectToAction(nameof(Index));
        }

        var command = new UpdateRoleCommand(model.Id.Value, model.Name, model.Description);
        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            ModelState.AddModelError(string.Empty, result.Error.Name);
            return View("~/Areas/Admin/Features/Role/Views/Edit.cshtml", model);
        }

        TempData["Success"] = $"Role '{model.Name}' updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// Deletes a role.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new DeleteRoleCommand(id);
        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            TempData["Error"] = result.Error.Name;
        }
        else
        {
            TempData["Success"] = "Role deleted successfully.";
        }

        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// Displays role permissions management page.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Permissions(Guid id)
    {
        var result = await _mediator.Send(new GetRoleWithPermissionsQuery(id));

        if (result.IsFailure)
        {
            TempData["Error"] = result.Error.Name;
            return RedirectToAction(nameof(Index));
        }

        var role = result.Value;
        var model = _BuildPermissionsViewModel(role);

        return View("~/Areas/Admin/Features/Role/Views/Permissions.cshtml", model);
    }

    /// <summary>
    /// Updates role permissions.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Permissions(Guid id, List<string> permissions)
    {
        var command = new UpdateRolePermissionsCommand(id, permissions);
        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            TempData["Error"] = result.Error.Name;
        }
        else
        {
            TempData["Success"] = "Permissions updated successfully.";
        }

        return RedirectToAction(nameof(Permissions), new { id });
    }

    /// <summary>
    /// Builds permissions view model with grouped permissions.
    /// </summary>
    private static RolePermissionsViewModel _BuildPermissionsViewModel(
        Application.Common.Abstractions.Identity.Models.RoleDto role)
    {
        var assignedPermissions = role.Permissions?.ToHashSet() ?? [];

        // Group permissions by resource using fully qualified name to avoid conflict with method
        var groups = Application.Features.Identities.Roles.Permissions.Admin
            .GroupBy(p => p.Resource)
            .Select(g => new PermissionGroupViewModel
            {
                Resource = g.Key,
                Permissions = g.Select(p => new PermissionItemViewModel
                {
                    Name = p.Name,
                    DisplayName = p.Description,
                    IsAssigned = assignedPermissions.Contains(p.Name)
                }).ToList()
            }).ToList();

        return new RolePermissionsViewModel
        {
            RoleId = role.Id,
            RoleName = role.Name,
            IsDefault = Roles.IsDefault(role.Name),
            PermissionGroups = groups
        };
    }
}
