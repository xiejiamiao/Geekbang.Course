using System;
using System.Collections.Generic;
using System.Text;
using Dimsum.Domain.OrderAggregate;
using Dimsum.Infrastructure.Core;

namespace Dimsum.Infrastructure.Repositories.Interfaces
{
    public interface IOrderRepository : IRepository<Order, long>
    {
    }
}
