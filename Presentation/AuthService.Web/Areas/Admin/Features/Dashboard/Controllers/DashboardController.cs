/**
 * DashboardController displays admin dashboard.
 *
 * <p>Entry point for Admin area after login.</p>
 */
namespace AuthService.Web.Areas.Admin.Features.Dashboard.Controllers;

using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Controller for admin dashboard.
/// </summary>
public class DashboardController : AdminBaseController
{
    /// <summary>
    /// Displays the admin dashboard.
    /// </summary>
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }
}
