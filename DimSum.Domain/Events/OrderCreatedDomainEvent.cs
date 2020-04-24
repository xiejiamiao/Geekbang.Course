using DimSum.Domain.Abstractions;
using DimSum.Domain.OrderAggregate;

namespace DimSum.Domain.Events
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
