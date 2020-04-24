using System;
using System.Collections.Generic;
using System.Text;
using DimSum.Domain.OrderAggregate;
using DimSum.Infrastructure.Core;

namespace DimSum.Infrastructure.Repositories
{
    public interface IOrderRepository : IRepository<Order, long>
    {
    }
}
