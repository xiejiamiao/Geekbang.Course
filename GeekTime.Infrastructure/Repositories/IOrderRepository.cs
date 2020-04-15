using System;
using System.Collections.Generic;
using System.Text;
using GeekTime.Domain.OrderAggregate;
using GeekTime.Infrastructure.Core;

namespace GeekTime.Infrastructure.Repositories
{
    public interface IOrderRepository : IRepository<Order, long>
    {
    }
}
