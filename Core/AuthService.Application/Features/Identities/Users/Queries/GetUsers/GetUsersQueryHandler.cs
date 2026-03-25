/**
 * GetUsersQueryHandler processes GetUsersQuery.
 *
 * <p>Retrieves all users via IIdentityUserService abstraction.</p>
 */
namespace AuthService.Application.Features.Identities.Users.Queries.GetUsers;

using AuthService.Application.Common.Abstractions.Identity;
using AuthService.Application.Common.Abstractions.Identity.Models;
using AuthService.Application.Common.Messaging;
using AuthService.Domain.Common;


/// <summary>
/// Handler for GetUsersQuery.
/// </summary>
public sealed class GetUsersQueryHandler : IQueryHandler<GetUsersQuery, List<UserDto>>
{
    private readonly IIdentityUserService _userService;

    public GetUsersQueryHandler(IIdentityUserService userService)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
    }

    public async Task<Result<List<UserDto>>> Handle(
        GetUsersQuery request,
        CancellationToken cancellationToken)
    {
        return await _userService.GetUsersAsync(cancellationToken);
    }
}
