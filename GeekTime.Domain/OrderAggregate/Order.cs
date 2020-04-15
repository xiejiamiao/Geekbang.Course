using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using GeekTime.Domain.Abstractions;
using GeekTime.Domain.Events;

namespace GeekTime.Domain.OrderAggregate
{
    public class Order : Entity<long>, IAggregateRoot
    {
        public string UserId { get; private set; }

        public string UserName { get; private set; }



        public int ItemCount { get; private set; }
        public Address Address { get; private set; }

        protected Order()
        {
            
        }

        public Order(string userId,string userName,int itemCount, Address address)
        {
            this.UserId = userId;
            this.UserName = userName;
            this.ItemCount = itemCount;
            Address = address;

            this.AddDomainEvent(new OrderCreatedDomainEvent(this));
        }

        public void ChangeAddress(Address address)
        {
            this.Address = address;
        }
    }
}
