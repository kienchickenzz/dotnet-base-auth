namespace AuthService.Application.Common.Exceptions;

using System.Net;


public class NotFoundException : DomainException
{
    public NotFoundException(string message)
        : base(message, null, HttpStatusCode.NotFound)
    {
    }
}
