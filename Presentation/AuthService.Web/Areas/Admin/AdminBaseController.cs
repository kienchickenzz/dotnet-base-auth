/**
 * Base controller for Admin area.
 *
 * <p>Requires Admin role for all derived controllers.</p>
 */
namespace AuthService.Web.Areas.Admin;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


/// <summary>
/// Base controller for Admin area with Admin role requirement.
/// </summary>
[Area("Admin")]
[Authorize(Roles = "Admin")]
public abstract class AdminBaseController : Controller
{
}
