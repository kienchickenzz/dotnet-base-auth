namespace AuthService.Application.Common.Messaging;

using MediatR;

using AuthService.Domain.Common;


public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{
}
