using System;
using System.Collections.Generic;
using System.Text;
using DimSum.Infrastructure.Core.Behaviors;
using DotNetCore.CAP;
using Microsoft.Extensions.Logging;

namespace DimSum.Infrastructure
{
    public class DomainContextTransactionBehavior<TRequest, TResponse> : TransactionBehavior<DomainContext, TRequest, TResponse>
    {
        public DomainContextTransactionBehavior(ILogger logger, DomainContext dbContext, ICapPublisher capBus) : base(logger, dbContext, capBus)
        {
        }
    }
}
