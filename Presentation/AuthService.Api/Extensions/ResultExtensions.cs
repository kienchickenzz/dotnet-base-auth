/**
 * ResultExtensions provides extension methods for Result pattern integration with ASP.NET Core.
 *
 * <p>Converts Error objects to standardized ProblemDetails responses (RFC 7807).</p>
 */
namespace AuthService.Api.Extensions;

using Microsoft.AspNetCore.Mvc;

using AuthService.Domain.Common;


/// <summary>
/// Extension methods for converting Result/Error to ActionResult responses.
/// </summary>
public static class ResultExtensions
{
    /// <summary>
    /// Converts an Error to a ProblemDetails ActionResult with specified status code.
    /// </summary>
    public static ActionResult ToProblemResult(this Error error, int statusCode)
    {
        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Type = "Error",
            Title = error.Code,
            Detail = error.Name
        };

        problemDetails.Extensions["errors"] = new[] { error };

        return new ObjectResult(problemDetails)
        {
            StatusCode = statusCode
        };
    }

    /// <summary>
    /// Converts an Error to a 400 Bad Request ProblemDetails response.
    /// </summary>
    public static ActionResult ToBadRequest(this Error error) =>
        error.ToProblemResult(StatusCodes.Status400BadRequest);

    /// <summary>
    /// Converts an Error to a 401 Unauthorized ProblemDetails response.
    /// </summary>
    public static ActionResult ToUnauthorized(this Error error) =>
        error.ToProblemResult(StatusCodes.Status401Unauthorized);

    /// <summary>
    /// Converts an Error to a 403 Forbidden ProblemDetails response.
    /// </summary>
    public static ActionResult ToForbidden(this Error error) =>
        error.ToProblemResult(StatusCodes.Status403Forbidden);

    /// <summary>
    /// Converts an Error to a 404 Not Found ProblemDetails response.
    /// </summary>
    public static ActionResult ToNotFound(this Error error) =>
        error.ToProblemResult(StatusCodes.Status404NotFound);

    /// <summary>
    /// Converts multiple Errors to a ProblemDetails ActionResult.
    /// </summary>
    public static ActionResult ToProblemResult(
        this IEnumerable<Error> errors,
        int statusCode,
        string title = "One or more errors occurred.")
    {
        var errorList = errors.ToList();
        var firstError = errorList.FirstOrDefault() ?? Error.None;

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Type = "Error",
            Title = firstError.Code,
            Detail = title
        };

        problemDetails.Extensions["errors"] = errorList;

        return new ObjectResult(problemDetails)
        {
            StatusCode = statusCode
        };
    }
}
