namespace AuthService.Application.Common.Exceptions;

using System.Net;


public class UnauthorizedException : DomainException
{
    public UnauthorizedException(string message)
        : base(message, null, HttpStatusCode.Unauthorized)
    {
    }
}
