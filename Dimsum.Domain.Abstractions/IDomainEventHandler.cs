using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace DimSum.Domain.Abstractions
{
    public interface IDomainEventHandler<in TDomainEvent> : INotificationHandler<TDomainEvent>
        where TDomainEvent : IDomainEvent
    {
    }
}
