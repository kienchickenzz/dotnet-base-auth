/**
 * GetUserByIdQuery retrieves a single user by Id.
 *
 * <p>Processed by GetUserByIdQueryHandler.</p>
 */
namespace AuthService.Application.Features.Identities.Users.Queries.GetUserById;

using AuthService.Application.Common.Abstractions.Identity.Models;
using AuthService.Application.Common.Messaging;


/// <summary>
/// Query to get user by Id.
/// </summary>
/// <param name="UserId">User's unique identifier.</param>
public sealed record GetUserByIdQuery(Guid UserId) : IQuery<UserDto>;
