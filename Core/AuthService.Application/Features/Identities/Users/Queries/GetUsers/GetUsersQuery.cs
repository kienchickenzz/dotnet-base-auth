/**
 * GetUsersQuery retrieves all users.
 *
 * <p>Processed by GetUsersQueryHandler.</p>
 */
namespace AuthService.Application.Features.Identities.Users.Queries.GetUsers;

using AuthService.Application.Common.Abstractions.Identity.Models;
using AuthService.Application.Common.Messaging;


/// <summary>
/// Query to get all users.
/// </summary>
public sealed record GetUsersQuery : IQuery<List<UserDto>>;
