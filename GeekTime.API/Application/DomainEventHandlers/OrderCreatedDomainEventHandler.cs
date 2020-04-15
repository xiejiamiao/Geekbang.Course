using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DotNetCore.CAP;
using GeekTime.API.Application.IntegrationEvents;
using GeekTime.Domain.Abstractions;
using GeekTime.Domain.Events;

namespace GeekTime.API.Application.DomainEventHandlers
{
    public class OrderCreatedDomainEventHandler:IDomainEventHandler<OrderCreatedDomainEvent>
    {
        private readonly ICapPublisher _capPublisher;

        public OrderCreatedDomainEventHandler(ICapPublisher capPublisher)
        {
            _capPublisher = capPublisher;
        }

        public async Task Handle(OrderCreatedDomainEvent notification, CancellationToken cancellationToken)
        {
            await _capPublisher.PublishAsync("OrderCreated", new OrderCreatedIntegrationEvent(notification.Order.Id), cancellationToken: cancellationToken);
        }
    }
}
