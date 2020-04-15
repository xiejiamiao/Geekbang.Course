using System;
using System.Collections.Generic;
using System.Text;
using DotNetCore.CAP;
using GeekTime.Infrastructure.Core.Behaviors;
using Microsoft.Extensions.Logging;

namespace GeekTime.Infrastructure
{
    public class DomainContextTransactionBehavior<TRequest, TResponse> : TransactionBehaviors<DomainContext, TRequest, TResponse>
    {
        public DomainContextTransactionBehavior(DomainContext dbContext, ICapPublisher capBus, ILogger logger) : base(dbContext, capBus, logger)
        {
        }
    }
}
