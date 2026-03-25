/**
 * GetUserByIdQueryHandler processes GetUserByIdQuery.
 *
 * <p>Retrieves user by Id via IIdentityUserService abstraction.</p>
 */
namespace AuthService.Application.Features.Identities.Users.Queries.GetUserById;

using AuthService.Application.Common.Abstractions.Identity;
using AuthService.Application.Common.Abstractions.Identity.Models;
using AuthService.Application.Common.Messaging;
using AuthService.Domain.Common;


/// <summary>
/// Handler for GetUserByIdQuery.
/// </summary>
public sealed class GetUserByIdQueryHandler : IQueryHandler<GetUserByIdQuery, UserDto>
{
    private readonly IIdentityUserService _userService;

    public GetUserByIdQueryHandler(IIdentityUserService userService)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
    }

    public async Task<Result<UserDto>> Handle(
        GetUserByIdQuery request,
        CancellationToken cancellationToken)
    {
        return await _userService.GetByIdAsync(request.UserId, cancellationToken);
    }
}
