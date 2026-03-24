namespace AuthService.Application.Common.Messaging;

using MediatR;

using AuthService.Domain.Common;


public interface IDomainEventHandler<TEvent> : INotificationHandler<TEvent>
    where TEvent : IDomainEvent
{
}
