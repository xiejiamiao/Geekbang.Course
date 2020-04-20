using System;
using System.Collections.Generic;
using System.Text;
using Dimsum.Domain.OrderAggregate;
using Dimsum.Infrastructure.Core;
using Dimsum.Infrastructure.Repositories.Interfaces;

namespace Dimsum.Infrastructure.Repositories.Implements
{
    public class OrderRepository : Repository<Order, long, DomainContext>, IOrderRepository
    {
        public OrderRepository(DomainContext context) : base(context)
        {
        }
    }
}
