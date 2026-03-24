namespace AuthService.Identity.Extensions;

using Microsoft.AspNetCore.Identity;

using AuthService.Domain.Common;


internal static class IdentityResultExtensions
{
    public static List<Error> GetErrors(this IdentityResult result)
    {
        return result.Errors
            .Select(error => new Error(error.Code, error.Description))
            .ToList();
    }
}