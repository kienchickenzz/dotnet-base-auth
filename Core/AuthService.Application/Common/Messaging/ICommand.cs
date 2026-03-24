namespace AuthService.Application.Common.Messaging;

using MediatR;

using AuthService.Domain.Common;


public interface ICommand : IRequest<Result>
{
}

public interface ICommand<TResponse> : IRequest<Result<TResponse>>
{
}
