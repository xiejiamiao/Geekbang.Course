using MediatR;

namespace Dimsum.Domain.Abstractions
{
    public interface IDomainEventHandler<TDomainEvent> : INotificationHandler<TDomainEvent>
        where TDomainEvent : IDomainEvent
    {
    }
}
