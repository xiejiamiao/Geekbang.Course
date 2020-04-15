using System;
using System.Collections.Generic;
using System.Text;
using Dimsum.Infrastructure.Core.Behaviors;
using DotNetCore.CAP;
using Microsoft.Extensions.Logging;

namespace Dimsum.Infrastructure
{
    public class DomainContextTransactionBehavior<TRequest,TResponse>:TransactionBehavior<DomainContext,TRequest,TResponse>
    {
        public DomainContextTransactionBehavior(DomainContext dbContext, ICapPublisher capBus, ILogger logger) : base(dbContext, capBus, logger)
        {
        }
    }
}
