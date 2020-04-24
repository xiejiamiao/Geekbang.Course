using System;
using System.Collections.Generic;
using System.Text;
using DimSum.Domain.Abstractions;
using DimSum.Domain.Events;

namespace DimSum.Domain.OrderAggregate
{
    public class Order : Entity<long>, IAggregateRoot
    {
        public string UserId { get; }
        public string UserName { get; }
        public int ItemCount { get; }
        public Address Address { get; }

        protected Order()
        {
        }

        public Order(string userId, string userName, int itemCount, Address address)
        {
            UserId = userId;
            UserName = userName;
            ItemCount = itemCount;
            Address = address;

            this.AddDomainEvent(new OrderCreatedDomainEvent(this));
        }
    }
}
