using System;
using System.Collections.Generic;
using System.Text;
using Dimsum.Domain.Abstractions;

namespace Dimsum.Domain.OrderAggregate
{
    public class Order : Entity<long>, IAggregateRoot
    {
        public string UserId { get; }
        public string OrderNo { get; }
        public Address Address { get; }
        public decimal Payment { get; }

        public Order(string userId, string orderNo, Address address, decimal payment)
        {
            UserId = userId;
            OrderNo = orderNo;
            Address = address;
            Payment = payment;
        }

    }
}
