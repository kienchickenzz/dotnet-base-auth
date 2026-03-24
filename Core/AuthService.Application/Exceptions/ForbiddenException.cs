namespace AuthService.Application.Common.Exceptions;

using System.Net;


public class ForbiddenException : DomainException
{
    public ForbiddenException(string message)
        : base(message, null, HttpStatusCode.Forbidden)
    {
    }
}
