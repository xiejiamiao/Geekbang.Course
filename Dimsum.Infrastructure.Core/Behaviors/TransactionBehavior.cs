using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dimsum.Infrastructure.Core.Extensions;
using DotNetCore.CAP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Dimsum.Infrastructure.Core.Behaviors
{
    public class TransactionBehavior<TDbContext, TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TDbContext : EFContext
    {
        private readonly TDbContext _dbContext;
        private readonly ICapPublisher _capBus;
        private readonly ILogger _logger;

        public TransactionBehavior(TDbContext dbContext,ICapPublisher capBus,ILogger logger)
        {
            _dbContext = dbContext?? throw new ArgumentNullException(nameof(dbContext));
            _capBus = capBus ?? throw new ArgumentNullException(nameof(capBus));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var response = default(TResponse);
            var typeName = request.GetGenericTypeName();

            try
            {
                if (_dbContext.HasActiveTransaction)
                {
                    return await next();
                }

                var strategy = _dbContext.Database.CreateExecutionStrategy();
                await strategy.ExecuteAsync(async () =>
                {
                    Guid transactionId;
                    await using var transaction = await _dbContext.BeginTransaction();
                    using (_logger.BeginScope("TransactionContext:{TransactionId}",transaction.TransactionId))
                    {
                        _logger.LogInformation("---- 开始事务 {TransactionId} ({@Command})", transaction.TransactionId, typeName, request);
                        response = await next();
                        _logger.LogInformation("---- 提交事务 {TransactionId} ({@Command})", transaction.TransactionId, typeName);

                        await _dbContext.CommitTransaction(transaction);
                        transactionId = transaction.TransactionId;

                    }
                });

                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "处理事务出错 {CommandName} ({@Command})", typeName, request);
                throw;
            }
        }
    }
}
