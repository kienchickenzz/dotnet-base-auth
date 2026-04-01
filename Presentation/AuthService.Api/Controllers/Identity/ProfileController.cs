/**
 * ProfileController handles self-service profile endpoints.
 *
 * <p>Allows authenticated users to manage their own profile and password.</p>
 */
namespace AuthService.Api.Controllers.Identity;

using MediatR;

using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using AuthService.Api.Extensions;
using AuthService.Application.Common.Extensions.Identity;
using AuthService.Application.Features.Identities.Users.Commands.Password;


/// <summary>
/// Self-service profile endpoints for authenticated users.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProfileController : ControllerBase
{
    private readonly ISender _sender;

    public ProfileController(ISender sender)
    {
        _sender = sender ?? throw new ArgumentNullException(nameof(sender));
    }

    /// <summary>
    /// Changes current user's password.
    /// </summary>
    [HttpPost("change-password")]
    public async Task<ActionResult> ChangePasswordAsync(
        ChangePasswordCommand command,
        CancellationToken cancellationToken)
    {
        var userIdString = User.GetUserId();
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
        {
            return Unauthorized();
        }

        if (userId != command.UserId)
        {
            return Forbid();
        }

        var result = await _sender.Send(command, cancellationToken);
        return result.IsSuccess ? NoContent() : result.Error.ToBadRequest();
    }
}
