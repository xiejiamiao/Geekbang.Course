using System;
using System.Collections.Generic;
using System.Text;
using GeekTime.Domain.Abstractions;
using GeekTime.Domain.OrderAggregate;

namespace GeekTime.Domain.Events
{
    public class OrderCreatedDomainEvent : IDomainEvent
    {
        public Order Order { get; }

        public OrderCreatedDomainEvent(Order order)
        {
            Order = order;
        }
    }
}
