using System;
using System.Collections.Generic;
using System.Text;
using DimSum.Domain.OrderAggregate;
using DimSum.Infrastructure.Core;

namespace DimSum.Infrastructure.Repositories
{
    public class OrderRepository : Repository<Order, long, DomainContext>, IOrderRepository
    {
        public OrderRepository(DomainContext dbContext) : base(dbContext)
        {
        }
    }
}
